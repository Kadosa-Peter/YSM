using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using Ysm.Core;

namespace Ysm.Data
{

    public class SubscriptionService : ISubscriptionService
    {
        public event EventHandler<SubscriptionServiceEventArgs> Started;
        public event EventHandler<SubscriptionServiceEventArgs> Finished;
        public event EventHandler<SubscriptionServiceEventArgs> Cancelled;
        public event EventHandler<SubscriptionServiceEventArgs> ProgressChanged;

        public bool IsRunning { get; private set; }

        private CancellationTokenSource _cancelationSource;
        private Stopwatch _stopwatch;

        public async void Run()
        {
            YouTubeService youTubeService = AuthenticationService.Default.YouTubeService;
            int all = await GetSubscriptionCountAsync(youTubeService);

            if (all != FileSystem.Service.ReadText().ToInt())
            {
                FileSystem.Service.WriteText(all.ToString());
            }
            else
            {
                Finished?.Invoke(this, new SubscriptionServiceEventArgs { NoNewSubscription = true });
                Logger.Log(MethodBase.GetCurrentMethod(), "The subscriptions have not changed");
                return;
            }

            IsRunning = true;

            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            _cancelationSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _cancelationSource.Token;

            List<Channel> currentChannels = Repository.Default.Channels.Get();
            List<Channel> newChannels = new List<Channel>();

            List<string> allChannels = new List<string>();
            List<string> removedChannels = new List<string>();

            Started?.Invoke(this, new SubscriptionServiceEventArgs { All = all });

            IProgress<int> progress = new Progress<int>(finished =>
            {
                ProgressChanged?.Invoke(this, new SubscriptionServiceEventArgs { All = all, Finished = finished });
            });

            try
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    string token = string.Empty;
                    int finished = 0;
                    while (token != null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        SubscriptionListResponse response = GetSubscriptionListResponse(youTubeService, token);
                        token = response.NextPageToken;

                        foreach (Subscription subscription in response.Items)
                        {
                            allChannels.Add(subscription.Snippet.ResourceId.ChannelId);

                            if (currentChannels.Any(x => x.Id == subscription.Snippet.ResourceId.ChannelId) == false)
                            {
                                newChannels.AddNotNull(CreateChannel(subscription));
                            }

                            finished++;
                            progress.Report(finished);
                        }
                    }

                    removedChannels = Repository.Default.Channels.Get().Select(x => x.Id).Except(allChannels).ToList();

                }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

#pragma warning disable 4014
                // OnFinished
                task.ContinueWith(t => OnFinished(newChannels, removedChannels), CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
                // OnCancelled
                task.ContinueWith(t => OnCancelled(newChannels), CancellationToken.None, TaskContinuationOptions.OnlyOnCanceled, TaskScheduler.FromCurrentSynchronizationContext());
#pragma warning restore 4014
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

        private void OnCancelled(List<Channel> channelsNew)
        {
            if (channelsNew.Count > 0)
            {
                Repository.Default.Channels.Insert(channelsNew);
            }

            _cancelationSource.Dispose();

            Cancelled?.Invoke(this, new SubscriptionServiceEventArgs { Channels = channelsNew });

            IsRunning = false;

            _stopwatch.Stop();
            Logger.Log(MethodBase.GetCurrentMethod(), $"Subscription Canceled: {_stopwatch.ElapsedMilliseconds / 1000}s");
        }

        private void OnFinished(List<Channel> channelsNew, List<string> removedChannels)
        {
            if (channelsNew.Count > 0)
            {
                Repository.Default.Channels.Insert(channelsNew);
            }

            if (removedChannels.Count > 0)
            {
                Repository.Default.Channels.Remove(removedChannels);
            }

            _cancelationSource.Dispose();

            Finished?.Invoke(this, new SubscriptionServiceEventArgs { Channels = channelsNew, Removed = removedChannels });

            IsRunning = false;

            _stopwatch.Stop();
            Logger.Log(MethodBase.GetCurrentMethod(), $"Subscription Finished: {_stopwatch.ElapsedMilliseconds / 1000}s");
        }

        private static SubscriptionListResponse GetSubscriptionListResponse(YouTubeService youTubeService, string token)
        {
            try
            {
                SubscriptionsResource.ListRequest request = youTubeService.Subscriptions.List("snippet");
                request.Order = SubscriptionsResource.ListRequest.OrderEnum.Alphabetical;
                request.MaxResults = 50;
                request.Mine = true;
                request.PageToken = token;

                return request.ExecuteAsync().Result;
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(SubscriptionService).Assembly.FullName,
                    ClassName = typeof(SubscriptionService).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion

                throw;
            }
        }

        private async Task<int> GetSubscriptionCountAsync(YouTubeService youTubeService)
        {
            try
            {
                SubscriptionsResource.ListRequest request = youTubeService.Subscriptions.List("contentDetails");
                request.Mine = true;

                SubscriptionListResponse response = await request.ExecuteAsync();

                if (response.PageInfo.TotalResults.HasValue)
                {
                    Logger.Log(MethodBase.GetCurrentMethod(), response.PageInfo.TotalResults.Value.ToString());

                    return response.PageInfo.TotalResults.Value;
                }

            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = GetType().Assembly.FullName,
                    ClassName = GetType().FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion
            }
            return -2;
        }

        private Channel CreateChannel(Subscription subscription)
        {
            SubscriptionSnippet snippet = subscription.Snippet;

            Channel channel = new Channel
            {
                Id = snippet.ResourceId.ChannelId,
                SubscriptionId = subscription.Id,
                Title = snippet.Title,
                Parent = Identifier.Empty
            };

            if (snippet.PublishedAt.HasValue)
            {
                channel.Date = snippet.PublishedAt.Value;
            }
            if (DownloadThumbnail(snippet.ResourceId.ChannelId, snippet.Thumbnails.Default__))
            {
                channel.Thumbnail = snippet.Thumbnails.Default__.Url;
            }
            else if (DownloadThumbnail(snippet.ResourceId.ChannelId, snippet.Thumbnails.Medium))
            {
                channel.Thumbnail = snippet.Thumbnails.Medium.Url;
            }
            else if (DownloadThumbnail(snippet.ResourceId.ChannelId, snippet.Thumbnails.High))
            {
                channel.Thumbnail = snippet.Thumbnails.High.Url;
            }
            else
            {
                // TODO: default thumbnail
            }

            return channel;
        }

        private bool DownloadThumbnail(string id, Thumbnail thumbnail)
        {
            if (thumbnail == null) return false;

            try
            {
                string fileName = Path.Combine(FileSystem.Thumbnails, id);
                fileName = fileName.InsertEnd(".jpg");

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(thumbnail.Url, fileName);
                }

                return true;
            }
            catch (WebException ex)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), $"Downloading thumbnail error: {ex.Status} - {id}");

                return false;
            }
            catch (Exception ex)
            {
                #region error

                Logger.Log(MethodBase.GetCurrentMethod(), $"Downloading thumbnail error: {id}");

                Error error = new Error
                {
                    AssemblyName = GetType().Assembly.FullName,
                    ClassName = GetType().FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion

                return false;

            }
        }

        public void Cancel()
        {
            _cancelationSource.Cancel(true);
        }
    }
}
