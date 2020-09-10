using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Ysm.Assets;
using Ysm.Assets.Menu;
using Ysm.Controls;
using Ysm.Core;
using Ysm.Data;
using Ysm.Models;
using Ysm.Windows;

namespace Ysm.Views
{
    public partial class PlaylistView
    {
        public ObservableCollection<Node> Nodes { get; set; }

        public ObservableCollection<object> SelectedNodes { get; set; }

        private Node _root;

        public PlaylistView()
        {
            InitializeComponent();

            ViewRepository.Add(this, nameof(PlaylistView));

            SetContextMenu();

            Nodes = new ObservableCollection<Node>();
            SelectedNodes = new ObservableCollection<object>();
        }

        private void PlaylistView_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadPlaylists();
        }

        private void LoadPlaylists()
        {
            SelectedNodes.Clear();
            Nodes.Clear();

            _root = CreateRoot();
            Nodes.Add(_root);

            List<Playlist> playLists = Repository.Default.Playlists.GetAll();

            foreach (Playlist playlist in playLists.OrderBy(x => x.Name))
            {
                if (playlist.Default) continue;

                Node node = CreatePlaylistNode(playlist);
                node.Rendered -= Node_Rendered; // átnevezés ne induljon el
                _root.Items.Add(node);
                _root.Count += playlist.Count();

                foreach (Channel channel in playlist.Channels)
                {
                    ChannelMapper.Add(channel);
                }
            }
        }

        private Node CreateRoot()
        {
            Category category = new Category();
            category.Id = Identifier.Empty;
            category.Title = Properties.Resources.Title_Playlists;

            Node node = new Node();
            node.Category = category;
            node.NodeType = NodeType.Root;

            node.CanDelete = false;
            node.CanRename = false;
            node.CanMove = false;
            node.CanCollapse = false;
            node.IsExpanded = true;

            return node;
        }

        private void NodeTree_OnSelectedItemsChanged(object sender, RoutedEventArgs e)
        {
            List<Video> videos = new List<Video>();

            List<Node> nodes = SelectedNodes.Cast<Node>().ToList();

            Kernel.Default.CanDelete = nodes.Any(x => x.NodeType == NodeType.Playlist);

            foreach (Node node in nodes)
            {
                if (node.NodeType == NodeType.Playlist)
                {
                    foreach (Video video in node.Playlist.Videos)
                    {
                        videos.Add(video);
                    }
                }
                else
                {
                    Playlist playlist = (node.Parent as Node)?.Playlist;

                    if (playlist != null)
                    {
                        videos.AddRange(playlist.Videos.Where(x => x.ChannelId == node.Id));
                    }
                }
            }

            ViewRepository.Get<VideoView>().ListVideos(videos);
        }

        private void NodeTree_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ExtendedTreeItem item = (e.OriginalSource as DependencyObject).GetParentOfType<ExtendedTreeItem>();

            if (item?.Header is Node node && node.NodeType == NodeType.Channel)
            {
                string channelUrl = UrlHelper.GetChannelUrl(node.Channel.Id);

                Process.Start(channelUrl);
            }
        }

        private void NodeTree_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                Node node = SelectedNodes.Cast<Node>().FirstOrDefault(x => x.NodeType == NodeType.Playlist);

                if (node != null)
                {
                    node.IsRenaming = true;
                }
            }
            if (e.Key == Key.F3)
            {
                CreatePlaylist();
            }
            if (e.Key == Key.Delete)
            {
                Delete();
            }
            else if (e.Key == Key.Escape)
            {
                SearchBox.Text = string.Empty;
            }
        }

        private void SearchBox_OnTextChanged(object sender, RoutedEventArgs e)
        {
            string text = SearchBox.Text;

            if (text.IsNull())
            {
                Kernel.Default.Search = false;

                foreach (NodeBase nodeBase in _root.Items)
                {
                    Node node = (Node)nodeBase;
                    node.Visibility = Visibility.Visible;
                }
            }
            else
            {
                Kernel.Default.Search = true;

                foreach (NodeBase nodeBase in _root.Items)
                {
                    Node node = (Node)nodeBase;

                    if (node.Playlist.Name.Contains(text, true))
                    {
                        node.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        node.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private Node CreatePlaylistNode(Playlist playlist, bool rename = true)
        {
            Node node = new Node();
            node.CanDelete = true;
            node.CanRename = true;
            node.CanCollapse = true;
            node.NodeType = NodeType.Playlist;
            node.Count = playlist.Videos.Count;
            node.Playlist = playlist;
            node.Title = playlist.Name;
            node.Parent = _root; // kell a Drop miatt

            if (playlist.Color.NotNull())
            {
                node.Color = playlist.Color.ToColor();
            }

            if (rename)
                node.Rendered += Node_Rendered;

            node.RenameEnded += Node_RenameEnded;
            node.Expanded += Node_Expanded;
            node.Collapsed += Node_Collapsed;

            if (playlist.Channels.Count > 0)
            {
                node.Items.Add(new Node());
            }

            return node;
        }

        private void Node_Collapsed(object sender, EventArgs e)
        {

        }

        private void Node_Expanded(object sender, EventArgs e)
        {
            if (sender is Node parent)
            {
                parent.Items.Clear();

                foreach (Channel channel in parent.Playlist.Channels)
                {
                    Node node = CreateChannelNode(channel, parent);

                    parent.Items.Add(node);
                }
            }
        }

        private void Node_Rendered(object sender, EventArgs e)
        {
            if (sender is Node node) node.IsRenaming = true;
        }

        private void Node_RenameEnded(object sender, EventArgs e)
        {
            if (sender is Node node)
            {
                Repository.Default.Playlists.Rename(node.Playlist.Id, node.Playlist.Name);

                node.Title = node.Playlist.Name;

                Sort();

                NodeTree.ForceFocus();
            }
        }

        private Node CreateChannelNode(Channel channel, Node parent)
        {
            Node node = new Node();
            node.Channel = channel;
            node.NodeType = NodeType.Channel;
            node.Parent = parent;
            node.CanCollapse = true;
            node.CanRename = false;
            node.Title = channel.Title;

            node.Count = parent.Playlist.Videos.Count(x => x.ChannelId == channel.Id);

            node.Icon = UrlHelper.GetIcon(channel.Id, 22, 22);

            return node;
        }

        public void Playlist_CreateOrRenamed(string id, string name)
        {
            Node node = _root.Items.Cast<Node>().FirstOrDefault(x => x.Playlist.Id == id);

            if (node != null)
            {
                node.Playlist.Name = name;
            }
            else
            {
                Playlist playlist = Repository.Default.Playlists.Get(id);

                if (playlist != null)
                {
                    node = CreatePlaylistNode(playlist, false);

                    _root.Items.Add(node);

                    Sort();
                }
            }
        }

        public void Video_Added(Video video, string playlistId)
        {
            if (_root.Items.Count == 0) return;

            _root.Count++;

            Node playlistNode = _root.Items.Cast<Node>().FirstOrDefault(x => x.Playlist.Id == playlistId);

            if (playlistNode != null)
            {
                if (playlistNode.Playlist.Channels.Any(x => x.Id == video.ChannelId))
                {
                    playlistNode.Playlist.Add(video);
                    playlistNode.Count++;

                    foreach (NodeBase nodeBase in playlistNode.Items)
                    {
                        Node node = (Node)nodeBase;

                        if (node.Id == video.ChannelId)
                        {
                            node.Count++;
                        }
                    }
                }
                else
                {
                    playlistNode.Playlist.Add(video);
                    playlistNode.Count++;

                    Channel channel = playlistNode.Playlist.Channels.FirstOrDefault(x => x.Id == video.ChannelId);

                    Node node = CreateChannelNode(channel, playlistNode);

                    node.Count = 1;

                    playlistNode.Items.Add(node);

                    Sort();
                }
            }
        }

        public Node FindPlaylist(string Id)
        {
            foreach (NodeBase nodeBase in _root.Items)
            {
                Node node = nodeBase as Node;

                if (node == null || node.Playlist == null) continue;

                if (node.Playlist.Id == Id)
                    return node;
            }

            return null;
        }

        public Node FindChannel(Node parent, string channelId)
        {
            foreach (NodeBase nodeBase in parent.Items)
            {
                Node child = (Node)nodeBase;

                if (child.NodeType == NodeType.Channel)
                {
                    if (child.Id == channelId)
                        return child;
                }
            }

            return null;
        }

        public void Sort()
        {
            List<Node> nodes = new List<Node>();
            nodes.AddRange(_root.Items.Cast<Node>());
            _root.Items.Clear();
            _root.Items.AddRange(nodes.OrderBy(x => x.Playlist.Name));
        }

        public void CreatePlaylist()
        {
            Playlist playlist = new Playlist();
            playlist.Name = Properties.Resources.Title_NewPlaylist;
            playlist.Id = Identifier.Sort;
            playlist.Default = false;

            Repository.Default.Playlists.Save(playlist);

            Node node = CreatePlaylistNode(playlist);

            _root.Items.Add(node);
        }

        public void Remove(string videoId, string channelId)
        {
            // Video removed in VideoView
            foreach (var nodeBase in _root.Items)
            {
                Node playlistNode = (Node) nodeBase;

                if (playlistNode != null && playlistNode.Playlist.Videos.Any(x => x.VideoId == videoId))
                {
                    playlistNode.Playlist.Remove(videoId);

                    playlistNode.Count--;

                    _root.Count--;

                    Node channelNode = FindChannel(playlistNode, channelId);

                    if (channelNode != null)
                    {
                        channelNode.Count--;

                        if (channelNode.Count == 0)
                        {
                            (channelNode.Parent as Node)?.Items.Remove(channelNode);
                        }
                    }

                    Repository.Default.Playlists.Save(playlistNode.Playlist);
                }
            }
        }

        public void Delete(string id)
        {
            Node node = FindPlaylist(id);

            if (node != null)
            {
                List<string> videos = node.Playlist.Videos.Select(x => x.VideoId).ToList();
                ViewRepository.Get<VideoView>().Remove(videos);

                _root.Count -= node.Playlist.Count();
                _root.Items.Remove(node);
            }
        }

        public bool CanRemove()
        {
            if (SelectedNodes == null)
                return false;

            return SelectedNodes.Cast<Node>().FirstOrDefault()?.CanDelete == true;
        }

        public bool CanEdit()
        {
            if (SelectedNodes == null)
                return false;

            return SelectedNodes.Cast<Node>().FirstOrDefault()?.CanRename == true;
        }

        public void Delete()
        {
            Node node = SelectedNodes.Cast<Node>().FirstOrDefault();
            if (node == null) return;

            if (node.NodeType == NodeType.Channel)
            {
                DeleteChannel(node);
            }
            else
            {
                DeletePlaylist(node);
            }

        }

        private void DeleteChannel(Node node)
        {
            try
            {
                if (Dialogs.OpenDialog(Messages.DeleteChannelFromPlaylist(node.Channel.Title)))
                {
                    Node parent = node.Parent as Node;
                    Playlist playlist = parent?.Playlist;

                    if (playlist != null)
                    {
                        playlist.Channels.RemoveAll(x => x.Id == node.Id);
                        playlist.Videos.RemoveAll(x => x.ChannelId == node.Id);
                        Repository.Default.Playlists.Save(playlist);
                        ViewRepository.Get<VideoView>().RemoveAll();

                        _root.Count -= node.Count;
                        parent.Count -= node.Count;

                        node.Remove();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), ex);
            }
        }

        private void DeletePlaylist(Node node)
        {
            try
            {
                void delete()
                {
                    Repository.Default.Playlists.Delete(node.Playlist.Id);
                    ViewRepository.Get<VideoView>().RemoveAll();
                    _root.Count -= node.Playlist.Count();
                    _root.Items.Remove(node);
                }

                if (node.Playlist.Videos.Count > 0)
                {
                    if (Dialogs.OpenDialog(Messages.DeletePlaylist(node.Playlist.Name)))
                    {
                        delete();
                    }
                }
                else
                {
                   delete();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), ex);
            }
        }

        public void Rename()
        {
            Node node = SelectedNodes.Cast<Node>().FirstOrDefault(x => x.NodeType == NodeType.Playlist);

            if (node != null)
            {
                node.IsRenaming = true;
            }
        }

        public void Cleanup()
        {
            Nodes.Clear();
            SelectedNodes.Clear();
        }

        public void Reset()
        {
            LoadPlaylists();
        }

        private void SetContextMenu()
        {
            if (AuthenticationService.Default.IsLoggedIn)
            {
                PlaylistNodeMenu menu = new PlaylistNodeMenu();
                NodeTree.ContextMenu = menu.Get();
            }
        }

        public void DefaultColor()
        {
            Node node = SelectedNodes.Cast<Node>().FirstOrDefault(x => x.NodeType == NodeType.Playlist);

            if (node != null)
            {
                node.Color = ColorHelper.GetDefaultFolderColor();

                node.Playlist.Color = null;

                Repository.Default.Playlists.Save(node.Playlist);
            }
        }

        public void ChangeColor()
        {
            Node node = SelectedNodes.Cast<Node>().FirstOrDefault(x => x.NodeType == NodeType.Playlist);

            if (node != null)
            {
                ColorPicker colorPicker = new ColorPicker(node);
                colorPicker.Owner = Application.Current.MainWindow;
                colorPicker.ShowDialog();
            }
        }

        public void Login()
        {
            LoadPlaylists();

            SetContextMenu();
        }

        public void Logout()
        {
            Nodes.Clear();
            SelectedNodes.Clear();
            _root = null;

            NodeTree.ContextMenu = null;
        }


    }
}



