using System.Windows;
using System.Windows.Input;

namespace Ysm.Downloader.Windows
{
    public partial class PlaylistWindow 
    {
        // 0 playlist, 1 video
        public int result { get; set; }

        public PlaylistWindow()
        {
            InitializeComponent();
        }

        private void Playlist_Click(object sender, RoutedEventArgs e)
        {
            result = 0;
            Close();
        }

        private void Video_Click(object sender, RoutedEventArgs e)
        {
            result = 1;
            Close();
        }

        private void Footer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
