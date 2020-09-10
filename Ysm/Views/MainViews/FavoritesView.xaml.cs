using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Ysm.Assets;
using Ysm.Assets.Caches;
using Ysm.Assets.Menu;
using Ysm.Controls;
using Ysm.Core;
using Ysm.Data;
using Ysm.Models;

namespace Ysm.Views
{
    public partial class FavoritesView
    {
        #region Variables & Properties

        public ObservableCollection<Node> Nodes { get; set; }

        public ObservableCollection<object> SelectedNodes { get; set; }

        public SearchEngine<Channel> Search { get; set; }

        private Node _root;

        private Playlist _playlist;

        private bool _removing;

        #endregion

        // CTOR
        public FavoritesView()
        {
            InitializeComponent();

            ViewRepository.Add(this, nameof(FavoritesView));

            Nodes = new ObservableCollection<Node>();
            SelectedNodes = new ObservableCollection<object>();

            Search = new SearchEngine<Channel>(SearchFunc);
            Search.Search += SearchEngine_OnSearch;

            FavoritesCache.Default.Added += Cache_Added;
            FavoritesCache.Default.Removed += Cache_Removed;

            FavoriesNodeMenu menu = new FavoriesNodeMenu();
            NodeTree.ContextMenu = menu.Get();
        }

        private void FavoritesView_OnLoaded(object sender, RoutedEventArgs e)
        {
            CreateList();
        }

        private void CreateList()
        {
            if (!AuthenticationService.Default.IsLoggedIn) return;

            Nodes.Clear();
            SelectedNodes.Clear();

            _playlist = Repository.Default.Playlists.Get("Favorites");

            CreateRoot();

            foreach (Channel channel in _playlist.Channels)
            {
                ChannelMapper.Add(channel);

                Node node = CreateChannel(channel, _root);
                node.Count = _playlist.Count(channel.Id);

                _root.Items.Add(node);
            }
        }

        public void CreateRoot()
        {
            Nodes.Clear();
            SelectedNodes.Clear();

            Category category = new Category
            {
                Id = Identifier.Empty,
                Title = Properties.Resources.Title_Favorites
            };

            _root = new Node
            {
                Category = category,
                NodeType = NodeType.Root,
                Parent = null,
                CanCollapse = false,
                IsExpanded = true,
                Title = category.Title,
                CanDelete = false,
                CanRename = false,
                CanMove = false,
                Count = _playlist.Count()
            };


            Nodes.Add(_root);
        }

        public Node CreateChannel(Channel channel, Node parent)
        {
            Node node = new Node
            {
                Channel = channel,
                NodeType = NodeType.Channel,
                Parent = parent,
                CanCollapse = true,
                CanRename = false,
                Title = channel.Title,
                Icon = UrlHelper.GetIcon(channel.Id, 22, 22)
            };

            return node;
        }

        private void NodeTree_OnSelectedItemsChanged(object sender, RoutedEventArgs e)
        {
            if (_root.IsSelected)
            {
                ViewRepository.Get<VideoView>().ListVideos(_playlist.Videos);
            }
            else
            {
                IEnumerable<string> selectedChannels = SelectedNodes.Cast<Node>().Select(x => x.Id);

                IEnumerable<Video> videos = _playlist.Videos.Where(x => selectedChannels.Contains(x.ChannelId));

                ViewRepository.Get<VideoView>().ListVideos(videos);
            }
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
            if (e.Key == Key.Delete)
            {
                RemoveChannel();
            }

        }

        public void RemoveAll()
        {
            _removing = true;

            foreach (Video video in _playlist.Videos)
            {
                FavoritesCache.Default.Remove(video.VideoId);
            }

            _playlist.Clear();
            Repository.Default.Playlists.Save(_playlist);

            ViewRepository.Get<VideoView>()?.RemoveAll();

            _root.Count = 0;
            _root.Items.Clear();

            _removing = false;
        }

        public void RemoveChannel()
        {
            List<Node> nodes = SelectedNodes.Cast<Node>().ToList();

            if (nodes.Count > 0)
            {
                _removing = true;

                foreach (Video video in _playlist.Videos)
                {
                    FavoritesCache.Default.Remove(video.VideoId);
                }

                foreach (Node node in nodes)
                {
                    _playlist.Videos.RemoveAll(x => x.ChannelId == node.Channel.Id);

                    _playlist.Channels.RemoveAll(x => x.Id == node.Channel.Id);
                }

                Repository.Default.Playlists.Save(_playlist);

                ViewRepository.Get<VideoView>()?.RemoveAll();

                foreach (Node node in nodes)
                {
                    _root.Items.Remove(node);
                }

                _root.Count = _playlist.Videos.Count;

                _removing = false;
            }
        }

        private void Cache_Removed(string id)
        {
            if (Kernel.Default.View != View.WatchLater || _removing) return;

            string channelId = Repository.Default.Videos.Get().FirstOrDefault(x => x.VideoId == id)?.ChannelId;
            if (channelId == null) return;

            _playlist.Remove(id);

            _root.Count--;

            if (Search.IsSearch)
            {
                foreach (Node searchNoden in Nodes)
                {
                    if (searchNoden.Channel.Id == channelId)
                    {
                        searchNoden.Count--;

                        if (searchNoden.Count == 0)
                        {
                            Nodes.Remove(searchNoden);
                            break;
                        }
                    }
                }
            }

            Node node = _root.Find(channelId);

            if (node != null)
            {
                node.Count--;

                if (node.Count == 0)
                {
                    node.Remove();
                }
            }

            ViewRepository.Get<VideoView>().Remove(id);
        }

        private void Cache_Added(string id)
        {
            if (Kernel.Default.View != View.Favorites) return;

            Video video = Repository.Default.Videos.Get().FirstOrDefault(x => x.VideoId == id);
            if (video == null) return;

            _playlist.Add(video);

            string channelId = video.ChannelId;

            Node n = _root.Find(channelId);

            _root.Count++;

            if (n == null)
            {
                Channel channel = Repository.Default.Channels.Get_By_Id(channelId);

                if (channel == null) return;

                Node node = CreateChannel(channel, _root);
                node.Count = _playlist.Count(channel.Id);
                _root.Items.Add(node);
                _root.Sort();
            }
            else
            {
                n.Count++;

                if (n.IsSelected)
                {
                    ViewRepository.Get<VideoView>().AddVideo(video);
                }
            }
        }

        private void SearchEngine_OnSearch(object sender, SearchEventArgs<Channel> e)
        {
            if (e.Reset)
            {
                Nodes.Clear();
                Nodes.Add((Node)Search.Catch);
                Search.Catch = null;
            }
            else
            {
                if (Search.Catch == null)
                    Search.Catch = _root;

                Nodes.Clear();

                foreach (Channel channel in e.Result)
                {
                    Node node = CreateChannel(channel, null);
                    node.Count = _playlist.Count(channel.Id);
                    Nodes.Add(node);
                }
            }
        }

        private IEnumerable<Channel> SearchFunc(string query)
        {
            return _playlist.Channels.Where(x => x.Title.Contains(query, true));
        }

        public void Login()
        {
            FavoriesNodeMenu menu = new FavoriesNodeMenu();
            NodeTree.ContextMenu = menu.Get();

            SearchBox.IsEnabled = true;
        }

        public void Logout()
        {
            NodeTree.ContextMenu = null;

            Nodes.Clear();
            SelectedNodes.Clear();
            SearchBox.IsEnabled = false;
            Search.Clear();
        }

        public void Cleanup()
        {
            _root.Items.Clear();
            _root = null;

            Search.Clear();

            _playlist = null;
        }

        public void Reset()
        {
            CreateList();
        }
    }
}
