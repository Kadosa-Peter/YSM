using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Ysm.Assets;
using Ysm.Assets.Trial;
using Ysm.Core;
using Ysm.Data;
using Ysm.Windows;

namespace Ysm.Views
{
    // https://stackoverflow.com/questions/11703833/dragmove-and-maximize
    public partial class HeaderView
    {
        private TrialTimer _trialTimer;

        private Point DragPoint;

        public HeaderView()
        {
            InitializeComponent();

            ViewRepository.Add(this, "HeaderView");

            AuthenticationService.Default.LoggedIn += (s, e) => SetLoginState();
            AuthenticationService.Default.LoggedOut += (s, e) => SetLoginState();

            SetLoginState();
        }

        private void SetLoginState()
        {
            if (AuthenticationService.Default.User == null)
            {
                LoginButton.Visibility = Visibility.Visible;
                UserButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                LoginButton.Visibility = Visibility.Collapsed;
                UserButton.Visibility = Visibility.Visible;

                UserButton.Content = AuthenticationService.Default.User.Name;
            }
        }

        private void Header_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                {
                    Point point = PointToScreen(e.MouseDevice.GetPosition(this));

                    if (point.Y - DragPoint.Y > 6)
                    {
                        Application.Current.MainWindow.Left = point.X - Application.Current.MainWindow.RestoreBounds.Width * 0.5;
                        Application.Current.MainWindow.Top = point.Y - ActualHeight / 2;

                        Application.Current.MainWindow.WindowState = WindowState.Normal;

                        Application.Current.MainWindow.DragMove();
                    }
                }
                else
                {
                    Application.Current.MainWindow.DragMove();
                }
            }
        }

        private void Header_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragPoint = PointToScreen(e.MouseDevice.GetPosition(this));

            if (e.ClickCount == 2)
            {
                if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                {
                    SystemCommands.RestoreWindow(Application.Current.MainWindow);
                    e.Handled = true;
                }
                else
                {
                    SystemCommands.MaximizeWindow(Application.Current.MainWindow);
                    e.Handled = true;
                }
            }
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            UserWindow window = new UserWindow();
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();

        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow window = new LoginWindow();
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
        }

        private void MenuItem_OnSubmenuOpened(object sender, RoutedEventArgs e)
        {
            Kernel.Default.MenuIsOpen = true;

            if (Licence.IsValid())
            {
                LicenceCommand.Visibility = Visibility.Collapsed;
            }

            if (AuthenticationService.Default.IsLoggedIn)
            {
                LogoutMenuButton.Visibility = Visibility.Visible;
                LoginMenuButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                LogoutMenuButton.Visibility = Visibility.Collapsed;
                LoginMenuButton.Visibility = Visibility.Visible;
            }

            if (Kernel.Default.View == View.Playlists)
            {
                btnCreateCategory.Visibility = Visibility.Collapsed;
                btnCreatePlaylist.Visibility = Visibility.Visible;
            }
            else
            {
                btnCreateCategory.Visibility = Visibility.Visible;
                btnCreatePlaylist.Visibility = Visibility.Collapsed;
            }
        }

        private void MenuItem_OnSubmenuClosed(object sender, RoutedEventArgs e)
        {
            Task.Delay(1000).GetAwaiter().OnCompleted(() => Kernel.Default.MenuIsOpen = false);
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.CloseToSystemTray)
            {
                if (Window.GetWindow(this) is MainWindow window)
                {
                    window.SystemTrayIcon.Visibility = Visibility.Visible;
                    window.ShowInTaskbar = false;
                    window.Hide();
                }
            }
            else
            {
                Application.Current.MainWindow?.Close();
            }
        }

        private void Minimize_OnClick(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.MinimizeToSystemTray)
            {
                if (Window.GetWindow(this) is MainWindow window)
                {
                    window.SystemTrayIcon.Visibility = Visibility.Visible;
                    window.ShowInTaskbar = false;
                    window.Hide();
                }
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Minimized;
            }
        }

        public void Trial(double hours)
        {
            _trialTimer = new TrialTimer(hours);
            _trialTimer.Elapsed += Timer_Elapsed;
            _trialTimer.Start();

            AppState.Visibility = Visibility.Visible;

            SetTrialText((int)Math.Round(hours));
        }

        private void Timer_Elapsed(double left)
        {
            SetTrialText((int)Math.Round(left));
        }

        private void SetTrialText(int hours)
        {
            int days = hours / 24 + 1;

            // 168/24+1 = 8;
            // 167/24+1 = 7;
            if (days == 8)
                days = 7;

            if (hours <= 24)
            {
                if (hours <= 1)
                {
                    AppState.Text = $"Trial: {hours} hour left";
                }
                else
                {
                    AppState.Text = $"Trial: {hours} hours left";
                }
            }
            else
            {
                AppState.Text = $"Trial: {days} days left";
            }
        }

        public void HideTrial()
        {
            _trialTimer?.Stop();
            AppState.Visibility = Visibility.Collapsed;
        }

        
    }
}
