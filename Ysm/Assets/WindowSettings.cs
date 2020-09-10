using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace Ysm.Assets
{
    public static class WindowSettings
    {
        public static void SetState(MainWindow window)
        {
            if (Settings.Default.StartOnSystemStartup && System.Environment.GetCommandLineArgs().Length > 1 && System.Environment.GetCommandLineArgs()[1] == "-on-system-startup")
            {
                if (Settings.Default.StartTaskbar)
                {
                    window.WindowState = WindowState.Minimized;

                }
                if (Settings.Default.StartNotificationArea)
                {
                    window.Hide();
                    window.ShowInTaskbar = false;
                    window.SystemTrayIcon.Visibility = Visibility.Visible;
                }
            }
        }

        public static void SetPosition(MainWindow window)
        {
            double width = SystemParameters.PrimaryScreenWidth;
            double height = SystemParameters.PrimaryScreenHeight;

            if (Settings.Default.FullScreen)
            {
                window.WindowState = WindowState.Maximized;
            }
            else if (width < 1300 || height < 800)
            {
                window.WindowState = WindowState.Maximized;
            }
            else if (Settings.Default.LastPosition)
            {
                if (Settings.Default.IsMaximized)
                {
                    window.WindowState = WindowState.Maximized;
                }
                else
                {
                    window.Left = Settings.Default.Left;
                    window.Top = Settings.Default.Top;

                    window.Width = Settings.Default.Width;
                    window.Height = Settings.Default.Height;
                }
            }
            else
            {
                window.Left = (width - Settings.Default.DefaultWidth) / 2;
                window.Top = (height - Settings.Default.DefaultHeight) / 2;
                window.Width = Settings.Default.DefaultWidth;
                window.Height = Settings.Default.DefaultHeight;
            }

            if (Settings.Default.StartTaskbar)
            {
                window.WindowState = WindowState.Minimized;
            }
            else if (Settings.Default.StartNotificationArea)
            {
                window.SystemTrayIcon.Visibility = Visibility.Visible;
                window.ShowInTaskbar = false;
                window.Hide();
            }
        }

        public static void SetLanguage(MainWindow window)
        {
            window.Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
        }
    }
}
