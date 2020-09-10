using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ysm.Core;

namespace Ysm.Data
{
    public class VideoService : IVideoService
    {
        public event EventHandler<VideoServiceEventArgs> Started;
        public event EventHandler<VideoServiceEventArgs> Finished;
        public event EventHandler<VideoServiceEventArgs> SegmentFinished;
        public event EventHandler<VideoServiceEventArgs> Cancelled;
        public event EventHandler<VideoServiceEventArgs> ProgressChanged;

        public bool IsRunning { get; private set; }

        private CancellationTokenSource _cancellationTokenSource;
        private ParallelOptions _options;
        private Stopwatch _stopwatch;

        public void Run(List<string> selectedChannels, SortType sort, bool downloadAll = false)
        {
            IsRunning = true;

            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            int all = selectedChannels?.Count ?? Repository.Default.Channels.Count();

            Started?.Invoke(this, new VideoServiceEventArgs { All = all });

            VideoError.Reset();

            _options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;

            List<string> currentVideos = Repository.Default.Videos.Get().Select(x => x.VideoId).ToList();
            List<Video> newVideos = new List<Video>();

            ConcurrentBag<string> errors = new ConcurrentBag<string>();

            try
            {
                IProgress<int> progress = new Progress<int>(finished =>
                {
                    ProgressChanged?.Invoke(this, new VideoServiceEventArgs { All = all, Finished = finished });
                });

                Task task = Task.Factory.StartNew(() =>
                {
                    List<List<string>> channelGroups = GetChannelGroups(selectedChannels, sort);

                    int finished = 0;

                    DateTime added = DateTime.Now;

                    foreach (List<string> list in channelGroups)
                    {
                        if (token.IsCancellationRequested)
                            break;

                        ConcurrentBag<Video> concurrent = new ConcurrentBag<Video>();

                        ConcurrentBag<string> channels = list.ToConcurrent();

                        Parallel.ForEach(channels, _options, (channel, state) =>
                        {
                            // In case of cancellation you must break the iteration first...
                            if (token.IsCancellationRequested)
                                state.Break();

                            List<Video> videos = DownloadVideos(channel, downloadAll);

                            if (videos != null)
                            {
                                videos.ForEach(x => x.Added = added);
                                concurrent.AddRange(videos);
                            }
                            else
                            {
                                errors.Add(channel);
                            }

                            Interlocked.Increment(ref finished);

                            progress.Report(finished);
                        });

                        List<Video> segment = new List<Video>();

                        foreach (Video video in concurrent)
                        {
                            if (currentVideos.Contains(video.VideoId) == false)
                            {
                                segment.Add(video);
                            }
                        }

                        ProcessSegment(segment);

                        newVideos.AddRange(segment);

                        // ...then throw a CancellationException.
                        token.ThrowIfCancellationRequested();
                    }

                    // **** ///
                }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                // Complete
                task.ContinueWith(t => Finalize(errors, newVideos, false), CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());

                // Canceled
                task.ContinueWith(t => Finalize(errors, newVideos, true), CancellationToken.None, TaskContinuationOptions.OnlyOnCanceled, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (AggregateException ae)
            {
                foreach (Exception exception in ae.InnerExceptions)
                {
                    Logger.Log(MethodBase.GetCurrentMethod(), exception.Message);
                }
            }
            catch (Exception e)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), e.Message);
            }
        }

        private List<Video> DownloadVideos(string channel, bool downloadAll)
        {
            List<Video> videos = null;

            //FeedDownloader max. 15 videót tölt le
            if (!downloadAll)
            {
                videos = FeedDownloader.DownladFeedAsync(channel);

                if (videos != null)
                {
                    VideoError.FeedError++;
                    return videos;
                }
            }

            videos = ExplodeDownloader.DownladVideos(channel, downloadAll);

            if (videos != null)
            {
                VideoError.ExplodeError++;
                return videos;
            }

            videos = ApiDownloader.Download(channel, downloadAll);

            if (videos != null)
            {
                VideoError.ApiError++;
                return videos;
            }

            return null;
        }

        private void Finalize(ConcurrentBag<string> errors, List<Video> new_videos, bool canceled)
        {
            if (canceled)
                Cancelled?.Invoke(this, new VideoServiceEventArgs { Videos = new_videos, Errors = errors.ToList() });
            else
                Finished?.Invoke(this, new VideoServiceEventArgs { Videos = new_videos, Errors = errors.ToList() });

            IsRunning = false;

            Logger.Log(MethodBase.GetCurrentMethod(), $"Explode Call: {VideoError.ExplodeError}");
            Logger.Log(MethodBase.GetCurrentMethod(), $"Api Call: {VideoError.ApiError}");
            Logger.Log(MethodBase.GetCurrentMethod(), $"FeedPharser Call: {VideoError.FeedError}");
            Logger.Log(MethodBase.GetCurrentMethod(), $"Errors: {errors.Count}");

            _stopwatch.Stop();
            Logger.Log(MethodBase.GetCurrentMethod(), $"Video Service: {_stopwatch.ElapsedMilliseconds / 1000}s");
        }

        private List<List<string>> GetChannelGroups(List<string> selectedChannels, SortType sort)
        {
            List<List<string>> group = new List<List<string>>();

            List<Channel> channels = Repository.Default.Channels.Get();

            if (selectedChannels != null)
            {
                // title
                if (sort == SortType.Title)
                {
                    List<string> list = new List<string>();
                    foreach (string id in channels.OrderBy(x => x.Title).Select(x => x.Id))
                    {
                        if (selectedChannels.Contains(id))
                            list.Add(id);
                    }

                    return list.ChunkBy(50);
                }
                // published
                else
                {
                    List<string> list = new List<string>();

                    foreach (string id in channels.OrderBy(x => x.Date).Select(x => x.Id))
                    {
                        if (selectedChannels.Contains(id))
                            list.Add(id);
                    }

                    group = list.ChunkBy(50);
                    group.Reverse();
                    return group;
                }
            }
            else
            {
                List<Category> categories = Repository.Default.Categories.Get().OrderBy(x => x.Title).ToList();

                if (categories.Count > 1)
                {
                    foreach (Category category in categories)
                    {
                        group.Add(channels.Where(x => x.Parent == category.Id).Select(x => x.Id).ToList());
                       
                    }
                    
                    // root
                    if (sort == SortType.Title)
                    {
                        List<List<string>> g = channels.Where(x => x.Parent == Identifier.Empty).OrderBy(x => x.Title).Select(x => x.Id).ToList().ChunkBy(50);
                        g.Reverse();
                        group.AddRange(g);

                    }
                    else
                    {
                        List<List<string>> g = channels.Where(x => x.Parent == Identifier.Empty).OrderBy(x => x.Date).Select(x => x.Id).ToList().ChunkBy(50);
                        g.Reverse();
                        group.AddRange(g);
                    }
                }
                else
                {
                    // title
                    if (sort == SortType.Title)
                    {
                        List<string> list = channels.OrderBy(x => x.Title).Select(x => x.Id).ToList();
                        group = list.ChunkBy(50);
                    }
                    // published
                    else
                    {
                        List<string> list = channels.OrderBy(x => x.Date).Select(x => x.Id).ToList();
                        group = list.ChunkBy(50);
                        group.Reverse();
                    }
                }
            }

            return group;
        }

        private void ProcessSegment(List<Video> videos)
        {
            // database update is in VideoServiceWrapper
            if (videos.Count > 0)
            {
                SegmentFinished?.Invoke(this, new VideoServiceEventArgs { Videos = videos });
            }
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel(true);
        }
    }
}
