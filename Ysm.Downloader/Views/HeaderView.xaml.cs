using System.Windows;
using System.Windows.Input;

namespace Ysm.Downloader.Views
{
    public partial class HeaderView
    {
        private Point DragPoint;

        public HeaderView()
        {
            InitializeComponent();
        }

        public void SetTitle(string title)
        {
            Title.Text = title;
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

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
    }
}
