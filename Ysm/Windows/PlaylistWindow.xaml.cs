using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Ysm.Assets;
using Ysm.Controls;
using Ysm.Core;
using Ysm.Data;
using Ysm.Models;
using Ysm.Views;

namespace Ysm.Windows
{
    public partial class PlaylistWindow
    {
        private List<Playlist> _playLists;

        private readonly Video _video;

        public PlaylistWindow(Video video)
        {
            InitializeComponent();

            _video = video;

            VideoTitle.Text = video.Title;
        }

        private void PlaylistWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadPlaylists();
        }

        private void LoadPlaylists()
        {
            _playLists = Repository.Default.Playlists.GetAll().OrderBy(x => x.Name).ToList();
            _playLists.ForEach(playlist => playlist.Clear());

            foreach (Playlist playlist in _playLists)
            {
                if (playlist.Default) continue;

                PlaylistNode node = CreatePlaylistNode(playlist);
                node.Rendered -= Node_Rendered; // átnevezés ne induljon el
                List.Items.Add(node);
            }
        }

        private void NewPlaylist_OnClick(object sender, RoutedEventArgs e)
        {
            Playlist playlist = new Playlist();
            playlist.Name = Properties.Resources.Title_NewPlaylist;
            playlist.Id = Identifier.Sort;
            playlist.Default = false;

            Repository.Default.Playlists.Save(playlist);

            PlaylistNode node = CreatePlaylistNode(playlist);

            List.Items.Add(node);

            List.Select(node);
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            Add();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void List_OnSelectedItemsChanged(object sender, RoutedEventArgs e)
        {
            AddButton.IsEnabled = List.SelectedItems.Count > 0;
        }

        private void List_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                Rename();
            }
            else if (e.Key == Key.Delete)
            {
                Delete();
            }
            else if (e.Key == Key.Enter)
            {
                Add();
            }
        }

        private void List_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ExtendedTreeItem item = (e.OriginalSource as DependencyObject).GetParentOfType<ExtendedTreeItem>();

            if (item?.Header is PlaylistNode node)
            {
                Repository.Default.Playlists.Insert(_video, node.Playlist.Id);

                ViewRepository.Get<PlaylistView>()?.Video_Added(_video, node.Playlist.Id);

                Close();
            }
        }

        private PlaylistNode CreatePlaylistNode(Playlist playlist)
        {
            PlaylistNode node = new PlaylistNode();
            node.CanDelete = true;
            node.CanRename = true;
            node.Title = playlist.Name;
            node.Playlist = playlist;
            node.Rendered += Node_Rendered;
            node.RenameEnded += Node_RenameEnded;

            return node;
        }

        private void Node_Rendered(object sender, System.EventArgs e)
        {
            if (sender is PlaylistNode node) node.IsRenaming = true;
        }

        private void Node_RenameEnded(object sender, System.EventArgs e)
        {
            if (sender is PlaylistNode node)
            {
                Repository.Default.Playlists.Rename(node.Playlist.Id, node.Playlist.Name);

                node.Title = node.Playlist.Name;

                ViewRepository.Get<PlaylistView>()?.Playlist_CreateOrRenamed(node.Playlist.Id, node.Playlist.Name);
            }
        }

        private void PlaylistWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void Window_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Add()
        {
            if (List.SelectedItems.First() is PlaylistNode node)
            {
                string playlistId = node.Playlist.Id;

                Playlist playlist = Repository.Default.Playlists.Get(playlistId);

                if (playlist == null)
                {
                    Repository.Default.Playlists.Insert(_video, playlistId);

                    ViewRepository.Get<PlaylistView>()?.Video_Added(_video, node.Playlist.Id);
                }
                else
                {
                    if (playlist.Videos.Any(x => x.VideoId == _video.VideoId))
                    {
                        string message = Properties.Resources.Title_PlaylistContainsVideo;
                        message = message.Replace("_XY_", playlist.Name);

                        Dialogs.OpenInfo(Messages.PlaylistContainsVideo(message, _video.Title));
                    }
                    else
                    {
                        Repository.Default.Playlists.Insert(_video, playlistId);

                        ViewRepository.Get<PlaylistView>()?.Video_Added(_video, node.Playlist.Id);
                    }
                }

                Close();
            }
        }

        private void Rename()
        {
            if (List.SelectedItems.Count > 0)
            {
                if (List.SelectedItems.First() is PlaylistNode node) node.IsRenaming = true;
            }
        }

        private void Delete()
        {
            if (List.SelectedItems.Count > 0)
            {
                if (List.SelectedItems.First() is PlaylistNode node)
                {
                    if (Dialogs.OpenDialog(Properties.Resources.Question_Playlist_Delete))
                    {
                        List.Items.Remove(node);

                        Repository.Default.Playlists.Delete(node.Playlist.Id);

                        ViewRepository.Get<PlaylistView>()?.Delete(node.Playlist.Id);
                    }
                }
            }
        }

        public void Reset()
        {
            List.Items.Clear();

            LoadPlaylists();
        }
    }
}
