using System;
using System.Windows;
using System.Windows.Input;
using Ysm.Core;
using Ysm.Data;
using Ysm.Views;

namespace Ysm.Windows
{
    public partial class NoteWindow
    {
        private TimeSpan _time;
        private Video _video;
        private Type _type;
        private Marker _entry;
        private MarkerGroup _markerGroup;

        public NoteWindow(Video video, int sec, string comment)
        {
            InitializeComponent();

            _time = new TimeSpan(0, 0, sec);
            _video = video;

            VideoTitle.Text = video.Title;

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
            Keyboard.Focus(Comment);
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
                Create_OnClick(null, null);
            }
        }

        private void Footer_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Create_OnClick(object sender, RoutedEventArgs e)
        {
            Marker entry = new Marker();
            entry.Comment = Comment.Text;
            entry.Time = _time;
            entry.Id = Identifier.Sort;

            MarkerGroup markerGroup = Repository.Default.Markers.Save(_video, entry);

            if (markerGroup != null)
            {
                ViewRepository.Get<MarkerView>()?.AddMarker(markerGroup, entry);
                ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView?.AddNote(markerGroup.Id, entry);
            }

            Close();
        }
    }
}
