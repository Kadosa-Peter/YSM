using System;
using System.Collections.Generic;
using System.Windows;
using Ysm.Core;
using Ysm.Data;
using Ysm.Properties;
using Ysm.Views;
using Ysm.Windows;

namespace Ysm.Assets
{
    public class VideoServiceWrapper
    {
        #region Singleton

        private static readonly Lazy<VideoServiceWrapper> _instance = new Lazy<VideoServiceWrapper>(() => new VideoServiceWrapper());

        public static VideoServiceWrapper Default => _instance.Value;

        private VideoServiceWrapper()
        {
            _cache = new List<Video>();
        }

        #endregion

        public event EventHandler<VideoServiceEventArgs> Started;
        public event EventHandler<VideoServiceEventArgs> Finished;
        public event EventHandler<VideoServiceEventArgs> Canceled;

        private VideoService _service;

        private List<Video> _cache;

        private bool _allowUpdate = true;

        private object _mutex = new object();

        public void Start(List<string> channels, bool downloadAll = false)
        {
            if (_service == null)
            {
                _service = new VideoService();
                _service.Started += Service_Started;
                _service.ProgressChanged += Service_ProgressChanged;
                _service.SegmentFinished += Service_SegmentFinished;
                _service.Cancelled += Service_Cancelled;
                _service.Finished += Service_Finished;
            }

            if (_service.IsRunning == false)
            {
                _service.Run(channels, Settings.Default.SubscriptionSort, downloadAll);
            }
        }

        private void Service_SegmentFinished(object sender, VideoServiceEventArgs e)
        {
            if (_allowUpdate)
            {
                Repository.Default.Videos.Insert(e.Videos);

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ViewRepository.Get<ChannelView>().UpdateChannelState(e.Videos);
                    ViewRepository.Get<FooterView>().IncreaseVideoCount(e.Videos.Count);
                    ViewRepository.Get<VideoView>().AddVideos(e.Videos);
                }));
            }
            else
            {
                lock (_mutex)
                {
                    _cache.AddRange(e.Videos);
                }
            }
        }

        private void Service_Started(object sender, VideoServiceEventArgs e)
        {
            Kernel.Default.VideoService = true;

            ViewRepository.Get<FooterView>().ServiceStarted(Resources.Title_DownloadingVideos, e.All);

            Started?.Invoke(this, e);
        }

        private void Service_ProgressChanged(object sender, VideoServiceEventArgs e)
        {
            ViewRepository.Get<FooterView>().ServiceProgressChanged(e.All, e.Finished);
        }

        private void Service_Finished(object sender, VideoServiceEventArgs e)
        {
            ProcessResult(e);

            Finished?.Invoke(this, e);
        }

        private void Service_Cancelled(object sender, VideoServiceEventArgs e)
        {
            ProcessResult(e, true);

            Canceled?.Invoke(this, e);
        }

        private void ProcessResult(VideoServiceEventArgs e, bool canceled = false)
        {
            Kernel.Default.VideoService = false;

            ViewRepository.Get<FooterView>().ServiceFinished();

            if (Settings.Default.AutoDownload)
            {
                DownloadTimer.Default.Reset();
            }

            if (!_allowUpdate) return;

            if (e.Videos.Count > 0 && !canceled)
            {
                NotifyWindow_Show(e.Videos);
                NotifyLayer_Show(e.Videos);
            }
        }
       
        public void Cancel()
        {
            if (_service != null && _service.IsRunning)
            {
                _service.Cancel();
            }
        }

        public void AllowUpdate()
        {
            _allowUpdate = true;

            ProcessCache();
        }

        public void ForbidUpdate()
        {
            _allowUpdate = false;
        }

        private void ProcessCache()
        {
            List<Video> videos = new List<Video>();

            lock (_mutex)
            {
                videos.AddRange(_cache);
                _cache.Clear();
            }

            if (videos.Count > 0)
            {
                Repository.Default.Videos.Insert(videos);

                ViewRepository.Get<ChannelView>().UpdateChannelState(videos);
                ViewRepository.Get<FooterView>().IncreaseVideoCount(videos.Count);
                ViewRepository.Get<VideoView>().AddVideos(videos);

                if (_service.IsRunning == false)
                {
                    NotifyLayer_Show(videos);
                    NotifyWindow_Show(videos);
                }
            }
        }

        private void NotifyLayer_Show(List<Video> videos)
        {
            if (videos.Count < 2)
            {
                ViewRepository.Get<MainViewHost>().ShowNotifyLayer(Resources.Text_NewVideo.Replace("xy", videos.Count.ToString()));
            }
            else
            {
                ViewRepository.Get<MainViewHost>().ShowNotifyLayer(Resources.Text_NewVideos.Replace("xy", videos.Count.ToString()));
            }
        }

        private void NotifyWindow_Show(List<Video> videos)
        {
            if (Settings.Default.ShowVideoNotifyWindow)
            {
                NotifyWindow notifyWindow = new NotifyWindow(videos);
                notifyWindow.Show();

            }
        }
    }
}
