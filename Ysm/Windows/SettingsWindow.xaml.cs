using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ysm.Windows
{
    public partial class SettingsWindow
    {
        private AppearanceSettings _appearanceSettings;
        private PlayerSettings _playerSettings;
        private DownloadsSettings _downloadsSettings;
        private NotificationSettings _notificationSettings;
        private SubscriptionSettings _subscriptionSettings;
        private VideoSettings _videosSettings;
        private HistorySettings _historySettings;

        private bool _loaded;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SettingsWindow2_OnLoaded(object sender, RoutedEventArgs e)
        {
            _loaded = true;

            AppearanceItem.IsSelected = true;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_loaded)
            {
                if (AppearanceItem.IsSelected)
                {
                    DisplayApplicationView();
                }
                else if (BrowserItem.IsSelected)
                {
                    DisplayBrowserView();
                }
                else if (DownloadsItem.IsSelected)
                {
                    DisplayDownloadsView();
                }
                else if (NotificationItem.IsSelected)
                {
                    DisplayNotificationView();
                }
                else if (SubscriptionsItem.IsSelected)
                {
                    DisplaySubscriptionView();
                }
                else if (VideosItem.IsSelected)
                {
                    DisplayVideosView();
                }
                else if (HistoryItem.IsSelected)
                {
                    DisplayHistoryView();
                }
            }
        }

        private void DisplayApplicationView()
        {
            if (_appearanceSettings == null)
                _appearanceSettings = new AppearanceSettings();

            SubView.Child = _appearanceSettings;
        }

        private void DisplayBrowserView()
        {
            if (_playerSettings == null)
                _playerSettings = new PlayerSettings();

            SubView.Child = _playerSettings;
        }

        private void DisplayDownloadsView()
        {
            if (_downloadsSettings == null)
                _downloadsSettings = new DownloadsSettings();

            SubView.Child = _downloadsSettings;
        }

        private void DisplayNotificationView()
        {
            if (_notificationSettings == null)
                _notificationSettings = new NotificationSettings();

            SubView.Child = _notificationSettings;
        }

        private void DisplaySubscriptionView()
        {
            if (_subscriptionSettings == null)
                _subscriptionSettings = new SubscriptionSettings();

            SubView.Child = _subscriptionSettings;
        }

        private void DisplayVideosView()
        {
            if (_videosSettings == null)
                _videosSettings = new VideoSettings();

            SubView.Child = _videosSettings;
        }

        private void DisplayHistoryView()
        {
            if (_historySettings == null)
                _historySettings = new HistorySettings();

            SubView.Child = _historySettings;
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SettingsWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Footer_OnMouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
