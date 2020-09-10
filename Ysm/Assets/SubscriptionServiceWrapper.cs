using System;
using System.Linq;
using Ysm.Core;
using Ysm.Data;
using Ysm.Properties;
using Ysm.Views;
using Ysm.Windows;

namespace Ysm.Assets
{
    public class SubscriptionServiceWrapper
    {
        #region Singleton

        private static readonly Lazy<SubscriptionServiceWrapper> _instance = new Lazy<SubscriptionServiceWrapper>(() => new SubscriptionServiceWrapper());

        public static SubscriptionServiceWrapper Default => _instance.Value;

        private SubscriptionServiceWrapper() { }

        #endregion

        public event EventHandler<SubscriptionServiceEventArgs> Started;
        public event EventHandler<SubscriptionServiceEventArgs> Finished;
        public event EventHandler<SubscriptionServiceEventArgs> Canceled;

        private SubscriptionService _service;

        private bool _startVideoService;

        public void Start(bool startVideoService = false)
        {
            _startVideoService = startVideoService;

            if (_service == null)
            {
                _service = new SubscriptionService();
                _service.Cancelled += Service_Cancelled;
                _service.Finished += Service_Finished;
                _service.Started += Service_Started;
                _service.ProgressChanged += _service_ProgressChanged;
            }

            if (_service.IsRunning == false)
            {
                _service.Run();
            }
        }

        private void Service_Started(object sender, SubscriptionServiceEventArgs e)
        {
            Kernel.Default.SubscriptionService = true;

            ViewRepository.Get<FooterView>().ServiceStarted(Resources.Title_DownloadingSubscriptions, e.All);

            Started?.Invoke(this, e);
        }

        private void _service_ProgressChanged(object sender, SubscriptionServiceEventArgs e)
        {
            ViewRepository.Get<FooterView>().ServiceProgressChanged(e.All, e.Finished);
        }

        private void Service_Finished(object sender, SubscriptionServiceEventArgs e)
        {
            ProcessResult(e);

            Finished?.Invoke(this, e);
        }

        private void Service_Cancelled(object sender, SubscriptionServiceEventArgs e)
        {
            ProcessResult(e, true);

            Canceled?.Invoke(this, e);
        }

        private void ProcessResult(SubscriptionServiceEventArgs e, bool canceled = false)
        {
            Kernel.Default.SubscriptionService = false;

            ViewRepository.Get<FooterView>().ServiceFinished();

            bool reset = true;

            if (e.Channels?.Count > 0)
            {
                ViewRepository.Get<ChannelView>().Reset();

                reset = false;

                ViewRepository.Get<FooterView>().IncreaseSubscriptionCount(e.Channels.Count);

                if (!canceled)
                {
                    if (Settings.Default.ShowSubscriptionNotifyWindow)
                    {
                        NotifyWindow notifyWindow = new NotifyWindow(e.Channels);
                        notifyWindow.Show();
                    }

                    if (_startVideoService)
                    {
                        VideoServiceWrapper.Default.Start(e.Channels.Select(x => x.Id).ToList());
                    }
                }

                ChannelMapper.Add(e.Channels);

                if (e.Channels.Count < 2)
                {
                    ViewRepository.Get<MainViewHost>().ShowNotifyLayer(Resources.Text_NewSubscription.Replace("xy", e.Channels.Count.ToString()));
                }
                else
                {
                    ViewRepository.Get<MainViewHost>().ShowNotifyLayer(Resources.Text_NewSubscriptions.Replace("xy", e.Channels.Count.ToString()));
                }
            }

            if (e.Removed?.Count > 0)
            {
                if (reset)
                {
                    ViewRepository.Get<ChannelView>().Reset();
                }

                foreach (string id in e.Removed)
                {
                   ViewRepository.Get<VideoView>().RemoveByChannelId(id);
                }

                ViewRepository.Get<FooterView>().UpdateCount();
            }

           
        }

        public void Cancel()
        {
            if (_service != null && _service.IsRunning)
            {
                _service.Cancel();
            }
        }
    }
}
