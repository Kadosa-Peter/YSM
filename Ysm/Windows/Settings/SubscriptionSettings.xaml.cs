using System.Windows;
using Ysm.Assets;

namespace Ysm.Windows
{
    public partial class SubscriptionSettings
    {
        public SubscriptionSettings()
        {
            InitializeComponent();

            Minutes.Text = Settings.Default.AutoDownloadMinutes.ToString();
        }

        private void AutoMarkAllWatched_OnClick(object sender, RoutedEventArgs e)
        {
            if (AutoMarkAllWatched.IsChecked == true)
            {
                AskMarkAllWatched.IsChecked = false;
            }
        }

        private void AskMarkAllWatched_OnClick(object sender, RoutedEventArgs e)
        {
            if (AskMarkAllWatched.IsChecked == true)
            {
                AutoMarkAllWatched.IsChecked = false;
            }
        }

        private void Decrease_OnClick(object sender, RoutedEventArgs e)
        {
            int minutes = int.Parse(Minutes.Text);

            if (minutes > 10)
            {
                minutes--;
                Settings.Default.AutoDownloadMinutes = minutes;
                Minutes.Text = minutes.ToString();
            }
        }

        private void Increase_OnClick(object sender, RoutedEventArgs e)
        {
            int minutes = int.Parse(Minutes.Text);

            if (minutes < 90)
            {
                minutes++;
                Minutes.Text = minutes.ToString();
            }
        }

        private void SubscriptionSettings_OnUnloaded(object sender, RoutedEventArgs e)
        {
            int minutes = int.Parse(Minutes.Text);

            if (Settings.Default.AutoDownloadMinutes != minutes)
            {
                Settings.Default.AutoDownloadMinutes = minutes;

                DownloadTimer.Default.Start();
            }
        }
    }
}
