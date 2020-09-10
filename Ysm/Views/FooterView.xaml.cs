using System.Windows;
using System.Windows.Input;
using Ysm.Assets;
using Ysm.Core;
using Ysm.Data;

namespace Ysm.Views
{
    public partial class FooterView
    {
        private int _subscriptionCount;
        private int _videoCount;

        public FooterView()
        {
            InitializeComponent();

            ViewRepository.Add(this, nameof(FooterView));

            Kernel.Default.PropertyChanged += Kernel_PropertyChanged;
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SubscriptionDisplayMode")
            {
                if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.ActiveSubscriptions)
                {
                    SubscriptionViewMode_Toggle.Content = Properties.Resources.Button_ShowAllSubscriptions;
                    //SubscriptionViewMode_Toggle.Content = "Subscriptions: All";
                }
                else
                {
                    SubscriptionViewMode_Toggle.Content = Properties.Resources.Button_ShowSubscriptionsWithVideo;
                    //SubscriptionViewMode_Toggle.Content = "Subscriptions: Unwached"; ;
                }
            }

            if (e.PropertyName == "VideoDisplayMode")
            {
                if (Settings.Default.VideoDisplayMode == VideoDisplayMode.UnwatchedVideos)
                {
                    VideoViewMode_Toggle.Content = Properties.Resources.Button_AllVideos;
                    //VideoViewMode_Toggle.Content = "Videos: All";
                }
                else
                {
                    VideoViewMode_Toggle.Content = Properties.Resources.Button_UnwatchedVideos;
                    //VideoViewMode_Toggle.Content = "Videos: Unwatched";
                }
            }
        }

        private void Kernel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "View")
            {
                if (Kernel.Default.View != View.Subscriptions)
                {
                    SubscriptionViewMode_Toggle.IsEnabled = false;
                    VideoViewMode_Toggle.IsEnabled = false;
                }
                else
                {
                    SubscriptionViewMode_Toggle.IsEnabled = true;
                    VideoViewMode_Toggle.IsEnabled = true;
                }
            }
        }

        private void FooterView_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (AuthenticationService.Default.IsLoggedIn)
            {
                CountHost.Visibility = Visibility.Visible;
                ViewCommandHost.Visibility = Visibility.Visible;

                UpdateCount();
            }
            else
            {
                CountHost.Visibility = Visibility.Collapsed;
                ViewCommandHost.Visibility = Visibility.Collapsed;
            }

            if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.ActiveSubscriptions)
            {
                SubscriptionViewMode_Toggle.Content = Properties.Resources.Button_ShowAllSubscriptions;
            }
            else
            {
                SubscriptionViewMode_Toggle.Content = Properties.Resources.Button_ShowSubscriptionsWithVideo;
            }

            if (Settings.Default.VideoDisplayMode == VideoDisplayMode.UnwatchedVideos)
            {
                VideoViewMode_Toggle.Content = Properties.Resources.Button_AllVideos;
            }
            else
            {
                VideoViewMode_Toggle.Content = Properties.Resources.Button_UnwatchedVideos;
            }
        }

        public void UpdateCount()
        {
            _subscriptionCount = Repository.Default.Channels.Count();
            _videoCount = Repository.Default.Videos.UnwatchedCount();

            CountLabel.Text = $"{_subscriptionCount}\\{_videoCount}";
        }

        public void IncreaseVideoCount(int i)
        {
            _videoCount += i;
            CountLabel.Text = $"{_subscriptionCount}\\{_videoCount}";
        }

        public void DecreaseVideoCount(int i)
        {
            _videoCount -= i;
            CountLabel.Text = $"{_subscriptionCount}\\{_videoCount}";
        }

        public void DecreaseSubscriptionCount(int i)
        {
            _subscriptionCount -= i;
            CountLabel.Text = $"{_subscriptionCount}\\{_videoCount}";
        }

        public void IncreaseSubscriptionCount(int i)
        {
            _subscriptionCount += i;
            CountLabel.Text = $"{_subscriptionCount}\\{_videoCount}";
        }

        // Services
        public void ServiceStarted(string service, double all)
        {
            ServiceLabel.Content = service;
            ProgressLabel.Text = $"{all}\\0";

            ServiceProgressBar.Value = 0;
            ServiceProgressBar.Visibility = Visibility.Visible;

            ServiceHost.Visibility = Visibility.Visible;
        }

        public void ServiceProgressChanged(double all, double finished)
        {
            ProgressLabel.Text = $"{all}\\{finished}";

            double percent = finished / (all / 100);
            ServiceProgressBar.Value = percent;
        }

        public void ServiceFinished()
        {
            ServiceProgressBar.Value = 0;
            ServiceProgressBar.Visibility = Visibility.Collapsed;

            ServiceHost.Visibility = Visibility.Collapsed;
        }

        private void CancelService_OnClick(object sender, RoutedEventArgs e)
        {
            VideoServiceWrapper.Default.Cancel();
            SubscriptionServiceWrapper.Default.Cancel();
        }

        // Change video & Subscription view
        private void ChangeSubscriptionViewMode_OnClick(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.ActiveSubscriptions)
            {
                Settings.Default.SubscriptionDisplayMode = SubscriptionDisplayMode.AllSubscriptions;
                //SubscriptionViewMode_Toggle.Content = Properties.Resources.Button_ShowSubscriptionsWithVideo;
            }
            else
            {
                Settings.Default.SubscriptionDisplayMode = SubscriptionDisplayMode.ActiveSubscriptions;
                //SubscriptionViewMode_Toggle.Content = Properties.Resources.Button_ShowAllSubscriptions;
            }
        }

        private void ChangeVideoViewMode_OnClick(object sender, RoutedEventArgs e)
        {
            // 0 => show only unwatched videos, 1 => show all videos
            // show all videos
            if (Settings.Default.VideoDisplayMode == VideoDisplayMode.UnwatchedVideos)
            {
                //VideoViewMode_Toggle.Content = Properties.Resources.Button_UnwatchedVideos;
                Settings.Default.VideoDisplayMode = VideoDisplayMode.AllVideos;
            }
            else
            {
                //VideoViewMode_Toggle.Content = Properties.Resources.Button_AllVideos;
                Settings.Default.VideoDisplayMode = VideoDisplayMode.UnwatchedVideos;
            }
        }

        // Autoplay/Shuffle/Repeat All
        public void SetAutoplay(Visibility visibility)
        {
            AutoplayHost.Visibility = visibility;
            AutoplayLabel.Visibility = visibility;
        }

        public void SetShuffle(Visibility visibility)
        {
            ShuffleLabel.Visibility = visibility;
        }

        public void SetRepeatAll(Visibility visibility)
        {
            RepeatAllLabel.Visibility = visibility;
        }


        private void Autoplay_OnClick(object sender, MouseButtonEventArgs e)
        {
            ViewRepository.Get<VideoView>().PlayEngine.AutoPlay = false;
            ViewRepository.Get<VideoView>().PlayEngine.Shuffle = false;
            ViewRepository.Get<VideoView>().PlayEngine.Repeat = false;

            AutoplayHost.Visibility = Visibility.Collapsed;
            AutoplayLabel.Visibility = Visibility.Collapsed;
        }

        private void Shuffle_OnClick(object sender, MouseButtonEventArgs e)
        {
            ViewRepository.Get<VideoView>().PlayEngine.Shuffle = false;
            ShuffleLabel.Visibility = Visibility.Collapsed;
        }

        private void RepeatAll_OnClick(object sender, MouseButtonEventArgs e)
        {
            ViewRepository.Get<VideoView>().PlayEngine.Repeat = false;
            RepeatAllLabel.Visibility = Visibility.Collapsed;
        }

        // Login & Logout
        public void Login()
        {
            UpdateCount();

            CountHost.Visibility = Visibility.Visible;
            ViewCommandHost.Visibility = Visibility.Visible;
        }

        public void Logout()
        {
            CountHost.Visibility = Visibility.Collapsed;
            ViewCommandHost.Visibility = Visibility.Collapsed;
            ServiceHost.Visibility = Visibility.Collapsed;
            ServiceProgressBar.Visibility = Visibility.Collapsed;
        }

        public void RemoveSubscription(bool started, bool finished, int all, int current)
        {
            if (started)
            {
                UnsubscriptionLabel.Visibility = Visibility.Visible;
                UnsubscriptionLabel.Text = $"Unsubscribe: {all}\\0";
            }

            if (finished)
            {
                UnsubscriptionLabel.Visibility = Visibility.Collapsed;
            }

            if (finished == false && started == false)
            {
                UnsubscriptionLabel.Text = $"Unsubscribe: {all}\\{current}";
            }

        }

    }
}
