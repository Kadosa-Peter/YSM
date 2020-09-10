using System;
using System.Windows;
using System.Windows.Input;
using Ysm.Core;
using Ysm.Data;
using Ysm.Views;

namespace Ysm.Windows
{
    public partial class MarkerUpdate
    {
        private string _marker;
        private string _entry;
        private string _comment;
        private int sec;
        private TimeSpan _time;
        
        public MarkerUpdate(string title, string marker, string entry, string comment, int sec)
        {
            InitializeComponent();

            _time = new TimeSpan(0, 0, sec);
            _entry = entry;
            _marker = marker;
            _comment = comment;

            VideoTitle.Text = title;

            if (_time.Hours > 0)
            {
                Time.Text = _time.ToString(@"h\:mm\:ss");
            }
            else
            {
                Time.Text = _time.ToString(@"mm\:ss");
            }

            if (comment.NotNull())
            {
                Comment.Text = comment;
                Comment.SelectAll();
                Comment.ForceFocus();
            }
        }


        private void BookmarkWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Comment.ForceFocus();
            Comment.SelectAll();
        }

        private void BookmarkWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            else if (e.Key == Key.Enter)
            {
                Update_OnClick(null, null);
            }
        }

        private void Update_OnClick(object sender, RoutedEventArgs e)
        {
            if (Comment.Text != _comment)
            {
                ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView.UpdateNote(_marker, _entry, Comment.Text);
                ViewRepository.Get<MarkerView>()?.UpdateMarker(_marker, _entry, Comment.Text);
                Repository.Default.Markers.Update(_marker, _entry, Comment.Text);
            }

            Close();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Footer_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

    }
}
