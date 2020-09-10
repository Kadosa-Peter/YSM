using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Ysm.Data;
using Ysm.Data.Comments;

namespace Ysm.Windows
{

    public partial class CommentWindow
    {
        private Point DragPoint;

        private Video _video;

        public ObservableCollection<Comment> Comments { get; set; }

        public CommentWindow(Video video)
        {
            InitializeComponent();

            Comments = new ObservableCollection<Comment>();

            _video = video;
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragPoint = PointToScreen(e.MouseDevice.GetPosition(this));

            if (e.ClickCount == 2)
            {
                if (WindowState == WindowState.Maximized)
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

        private void Header_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (WindowState == WindowState.Maximized)
                {
                    Point point = PointToScreen(e.MouseDevice.GetPosition(this));

                    if (point.Y - DragPoint.Y > 6)
                    {
                        Left = point.X - RestoreBounds.Width * 0.5;
                        Top = point.Y - ActualHeight / 2;

                        WindowState = WindowState.Normal;

                        DragMove();
                    }
                }
                else
                {
                    DragMove();
                }
            }
        }

        private async void CommentWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Comments.Clear();

            CommentThread commentThread = await CommentService.GetCommentsAsync(_video.VideoId);

            foreach (Comment comment in commentThread.Comments)
            {
               Comments.Add(comment);
            }
        }
    }
}
