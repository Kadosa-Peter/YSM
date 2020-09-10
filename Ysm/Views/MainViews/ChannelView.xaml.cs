using MoreLinq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
    // Channel.State => megnem nézett videók száma

    public partial class ChannelView
    {
        public ObservableCollection<Node> Nodes { get; set; }

        public ObservableCollection<object> SelectedNodes { get; set; }

        public SearchEngine<Channel> Search { get; set; }

        private Node _root;

        private string _channelId; // locate

        private string _videoId; // locate

        public ChannelView()
        {
            InitializeComponent();

            ViewRepository.Add(this, nameof(ChannelView));

            Search = new SearchEngine<Channel>(SearchFunc);
            Search.Search += Search_OnSearch;

            Nodes = new ObservableCollection<Node>();
            SelectedNodes = new ObservableCollection<object>();

            Settings.Default.PropertyChanged += ApplicationSettings_PropertyChanged;

            SetContextMenu();

            if (AuthenticationService.Default.IsLoggedIn)
            {
                CreateRoot();
            }

            RemoveSubscriptions.Changed += RemoveSubscriptions_Changed;
        }

        private void SetContextMenu()
        {
            if (AuthenticationService.Default.IsLoggedIn)
            {
                ChannelMenu channelMenu = new ChannelMenu();
                NodeTree.ContextMenu = channelMenu.Get();
            }
        }

        private void RemoveSubscriptions_Changed(object sender, RemoveSubscriptionsEventArgs e)
        {
            ViewRepository.Get<FooterView>().RemoveSubscription(e.Started, e.Finished, e.All, e.Current);
        }

        private void Search_OnSearch(object sender, SearchEventArgs<Channel> e)
        {
            void Cleanup()
            {
                Nodes.Clear();
                SelectedNodes.Unselect();
                Kernel.Default.IsRootOnlySelected = false;
                Kernel.Default.SelectedChannels?.Clear();
            }

            if (e.Reset)
            {
                List<string> selectedChannels = SelectedNodes
                    .Cast<Node>()
                    .Where(x => x.NodeType == NodeType.Channel)
                    .Select(x => x.Id).ToList();

                Cleanup();

                Kernel.Default.Search = false;
                Nodes.Add(_root);

                if (selectedChannels.Count == 1)
                {
                    Select(selectedChannels[0]);
                }
            }
            else
            {
                Cleanup();

                Kernel.Default.Search = true;
                foreach (Channel channel in e.Result)
                {
                    int channelState = Repository.Default.Channels.GetState(channel.Id);

                    if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.ActiveSubscriptions)
                        if (channelState == 0)
                            continue;

                    Node node = NodeFactory.CreateChannelNode(channel, null, channelState);

                    Nodes.Add(node);
                }
            }
        }

        private IEnumerable<Channel> SearchFunc(string query)
        {
            return Repository.Default.Channels.Search(query);
        }

        private void ChannelView_OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void ApplicationSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SubscriptionSort")
            {
                if (Settings.Default.SubscriptionSort == SortType.Title)
                {
                    ReorderNodesByTitle(_root);
                }
                else
                {
                    ReorderNodesByDate(_root);
                }
            }
            else if (e.PropertyName == "SubscriptionDisplayMode")
            {
                Reset();
            }
        }

        private void CategoryNode_Collapsed(object sender, EventArgs e)
        {
            // Drag Drop esetén a move-nál bezárhatjuk a parent nodot, de ekkor a kiválasztás lekerül a mozgatott node-ról
            if (Kernel.Default.IsDragDrop == false)
            {
                Node node = sender as Node;
                if (node == null) return;

                List<string> children = Repository.Default.Schema.GetChildren(node.Id);

                foreach (Node selectedNode in SelectedNodes.Cast<Node>().ToList())
                {
                    if (children.Contains(selectedNode.Id))
                    {
                        SelectedNodes.Remove(selectedNode);
                    }
                }

                if (node.HasItems)
                {
                    node.Items.Clear();
                    node.Items.Add(new Node());
                }
                else
                {
                    node.Items.Clear();
                }
            }
        }

        private void CategoryNode_Expanded(object sender, EventArgs e)
        {
            Node parent = sender as Node;
            if (parent == null) return;

            ListItems(parent);
        }

        public void ListItems(Node parent)
        {
            parent.Items.Clear();

            Task.Run(() =>
            {
                List<Channel> channels;

                if (Settings.Default.SubscriptionSort == SortType.Title)
                {
                    channels = Repository.Default.Channels.Get(parent.Id).OrderBy(x => x.Title).ToList();
                }
                else
                {
                    channels = Repository.Default.Channels.Get(parent.Id).OrderByDescending(x => x.Date).ToList();
                }

                List<Category> categories = Repository.Default.Categories.Get(parent.Id).ToList();

                return Tuple.Create(categories, channels);

            }).ContinueWith(t =>
            {
                foreach (Category category in t.Result.Item1)
                {
                    Node node = NodeFactory.CreateCategoryNode(category, parent);

                    node.Expanded += CategoryNode_Expanded;
                    node.Collapsed += CategoryNode_Collapsed;

                    parent.Items.Add(node);
                }

                foreach (Channel channel in t.Result.Item2)
                {
                    int channelState = Repository.Default.Channels.GetState(channel.Id);

                    if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.ActiveSubscriptions)
                        if (channelState == 0)
                            continue;

                    Node node = NodeFactory.CreateChannelNode(channel, parent, channelState);

                    parent.Items.Add(node);
                }

                Locate();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void NodeTree_OnSelectedItemsChanged(object sender, RoutedEventArgs e)
        {
            if (SelectedNodes.Count == 0) return; // possible?

            //Task.Run(() =>
            //{
            //    List<string> channels = new List<string>();

            //    List<Node> selectedNodes = SelectedNodes.Cast<Node>().ToList();

            //    if (selectedNodes.Contains(_root))
            //    {
            //        if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.AllSubscriptions)
            //        {
            //            channels.AddRange(Repository.Default.Channels.Get().Select(x => x.Id));
            //        }
            //        else
            //        {
            //            channels.AddRange(Repository.Default.GetActiveChannels());
            //        }
            //    }
            //    else
            //    {
            //        foreach (Node node in selectedNodes)
            //        {
            //            if (node.NodeType == NodeType.Channel)
            //            {
            //                channels.Add(node.Id);
            //            }
            //            else
            //            {
            //                if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.ActiveSubscriptions)
            //                {
            //                    List<string> activeChannels = Repository.Default.GetActiveChannels();

            //                    channels.AddRange(Repository.Default.Schema.GetChildren(node.Id).Where(x => activeChannels.Contains(x)));
            //                }
            //                else
            //                {
            //                    channels.AddRange(Repository.Default.Schema.GetChildren(node.Id));
            //                }
            //            }
            //        }
            //    }

            //    Kernel.Default.IsRootOnlySelected = selectedNodes.Count == 1 && selectedNodes.First().Id == Identifier.Empty;

            //    Kernel.Default.CanCut = selectedNodes.Any(x => x.CanMove);

            //    Kernel.Default.CanPaste = selectedNodes.Count == 1 && (selectedNodes.First().NodeType == NodeType.Category || selectedNodes.First().NodeType == NodeType.Root) && Move.NotEmpty;

            //    Kernel.Default.CanDelete = selectedNodes.First().CanDelete;

            //    Kernel.Default.CanRename = selectedNodes.First().CanRename;

            //    Kernel.Default.SelectedChannels = channels;

            //    Kernel.Default.SelectedVideoItem = null;

            //    return channels;

            //}).ContinueWith(t =>
            //{
            //    if (Search.IsReset())
            //        return;

            //    ViewRepository.Get<VideoView>().SelectedChannelChanged(t.Result);

            //}, TaskScheduler.FromCurrentSynchronizationContext());










            List<string> channels = new List<string>();

            List<Node> selectedNodes = SelectedNodes.Cast<Node>().ToList();

            if (selectedNodes.Contains(_root))
            {
                if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.AllSubscriptions)
                {
                    channels.AddRange(Repository.Default.Channels.Get().Select(x => x.Id));
                }
                else
                {
                    channels.AddRange(Repository.Default.Schema.GetActiveChannels());
                }
            }
            else
            {
                foreach (Node node in selectedNodes)
                {
                    if (node.NodeType == NodeType.Channel)
                    {
                        channels.Add(node.Id);
                    }
                    else
                    {
                        if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.ActiveSubscriptions)
                        {
                            List<string> activeChannels = Repository.Default.Schema.GetActiveChannels();

                            channels.AddRange(Repository.Default.Schema.GetChildren(node.Id).Where(x => activeChannels.Contains(x)));
                        }
                        else
                        {
                            channels.AddRange(Repository.Default.Schema.GetChildren(node.Id));
                        }
                    }
                }
            }

            Kernel.Default.IsRootOnlySelected = selectedNodes.Count == 1 && selectedNodes.First().Id == Identifier.Empty;

            Kernel.Default.CanCut = selectedNodes.Any(x => x.CanMove);

            Kernel.Default.CanPaste = selectedNodes.Count == 1 && (selectedNodes.First().NodeType == NodeType.Category || selectedNodes.First().NodeType == NodeType.Root) && Move.NotEmpty;

            Kernel.Default.CanDelete = selectedNodes.First().CanDelete;

            Kernel.Default.CanRename = selectedNodes.First().CanRename;

            Kernel.Default.SelectedChannels = channels;

            Kernel.Default.SelectedVideoItem = null;

            if (Search.IsReset())
                return;

            ViewRepository.Get<VideoView>().SelectedChannelChanged(channels);

        }

        private void NodeTree_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3)
            {
                CreateCategory();
                e.Handled = true;
            }
            else if (e.Key == Key.F2)
            {
                Rename();
                e.Handled = true;
            }
            if (e.Key == Key.Delete)
            {
                Delete();
                e.Handled = true;
            }
            else if (e.Key == Key.X && ModifierKeyHelper.IsCtrlDown)
            {
                Cut();
                e.Handled = true;
            }
            else if (e.Key == Key.V && ModifierKeyHelper.IsCtrlDown)
            {
                Paste();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                if (Kernel.Default.Search)
                {
                    SearchBox.Clear();
                }
                else
                {
                    ClearMove();
                }

                e.Handled = true;
            }
            else if (ModifierKeyHelper.IsShiftDown == false && e.Key == Key.Enter)
            {
                if (!Kernel.Default.IsRenaming)
                {
                    MarkWatched();
                    e.Handled = true;
                }
            }
        }

        private void NodeTree_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Add && ModifierKeyHelper.IsCtrlDown)
            {
                ExpandAll();
                e.Handled = true;
            }
            else if (e.Key == Key.Subtract && ModifierKeyHelper.IsCtrlDown)
            {
                CollapseAll();
                e.Handled = true;
            }
        }

        private void CollapseAll()
        {
            List<Node> nodes = _root.GetChildren();

            foreach (Node node in nodes)
            {
                if (node.NodeType == NodeType.Category)
                {
                    node.IsExpanded = false;
                }
            }
        }

        private void ExpandAll()
        {
            List<Node> nodes = _root.GetChildren();

            foreach (Node node in nodes)
            {
                if (node.NodeType == NodeType.Category && node.HasItems)
                {
                    node.IsExpanded = true;
                }
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

        private void CreateRoot()
        {
            _root = NodeFactory.CreateRoot();

            //await ListItems(_root).ContinueWith(x => Locate(), TaskScheduler.FromCurrentSynchronizationContext());
            ListItems(_root);

            Nodes.Clear();
            Nodes.Add(_root);
        }

        private void ReorderNodesByDate(Node node)
        {
            List<Node> channels = node.Items.Cast<Node>().Where(x => x.NodeType == NodeType.Channel).OrderByDescending(x => x.Channel.Date).ToList();
            List<Node> categories = node.Items.Cast<Node>().Where(x => x.NodeType == NodeType.Category).OrderBy(x => x.Category.Title).ToList();

            node.Items.Clear();

            node.Items.AddRange(categories);
            node.Items.AddRange(channels);

            foreach (Node category in categories)
            {
                if (category.IsExpanded)
                {
                    ReorderNodesByDate(category);
                }
            }
        }

        private void ReorderNodesByTitle(Node node)
        {
            List<Node> channels = node.Items.Cast<Node>().Where(x => x.NodeType == NodeType.Channel).OrderBy(x => x.Channel.Title).ToList();
            List<Node> categories = node.Items.Cast<Node>().Where(x => x.NodeType == NodeType.Category).OrderBy(x => x.Category.Title).ToList();

            node.Items.Clear();

            node.Items.AddRange(categories);
            node.Items.AddRange(channels);

            foreach (Node category in categories)
            {
                if (category.IsExpanded)
                {
                    ReorderNodesByTitle(category);
                }
            }
        }

        public void UpdateChannelState(List<Video> videos)
        {
            if (videos.Count > 0)
            {
                if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.ActiveSubscriptions)
                {
                    UpdateSubscriptions(videos);
                }
                else
                {
                    // collection modified exception
                    //_root.Count = Repository.Default.Videos.UnwatchedCount();
                    _root.Count += videos.Count;

                    IEnumerable<string> channels = videos.DistinctBy(x => x.ChannelId).Select(x => x.ChannelId);

                    foreach (string channel in channels)
                    {
                        try
                        {
                            Node node = _root.Find(channel);

                            if (node != null)
                            {
                                node.Count = Repository.Default.Videos.UnwatchedCount(node.Id);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Log(MethodBase.GetCurrentMethod(), e);
                        }
                    }

                    IEnumerable<Node> categories = _root.GetVisibleNodes().Where(x => x.NodeType == NodeType.Category);

                    foreach (Node node in categories)
                    {
                        node.Count = Repository.Default.Categories.GetState(node.Id);
                    }
                }
            }
        }

        private void UpdateSubscriptions(List<Video> videos)
        {
            List<string> channel_ids = new List<string>();
            foreach (Video video in videos)
            {
                if (channel_ids.Contains(video.ChannelId) == false)
                    channel_ids.Add(video.ChannelId);
            }
            // -----------

            List<Channel> channels = Repository.Default.Channels.Get().Where(x => channel_ids.Contains(x.Id)).ToList();
            // -----------

            List<string> category_ids = new List<string>();
            foreach (Channel channel in channels)
            {
                if (category_ids.Contains(channel.Parent) == false)
                    category_ids.Add(channel.Parent);
            }
            // -----------

            foreach (string id in category_ids)
            {
                Node category = _root.Find(id);
                if (category != null)
                {
                    category.Count = Repository.Default.Categories.GetState(category.Id);

                    if (category.IsExpanded)
                    {
                        List<Node> nodes = new List<Node>();

                        foreach (Channel channel in channels.Where(x => x.Parent == id))
                        {
                            Node node = category.Find(channel.Id);

                            if (node != null)
                            {
                                // mivel a channel már szerepel a kategóriában (van neki meg nem nézett videója) ezért csak frissítem
                                node.Count = Repository.Default.Videos.UnwatchedCount(channel.Id);
                            }
                            else
                            {
                                // a channel nem szerepel a kategóriában tehát létrehozok egy újat
                                nodes.Add(NodeFactory.CreateChannelNode(channel, category));
                            }
                        }

                        // clear - sort - add
                        nodes.AddRange(category.Items.Cast<Node>());
                        if (Settings.Default.SubscriptionSort == SortType.Title)
                        {
                            IOrderedEnumerable<Node> c = nodes.Where(x => x.NodeType == NodeType.Category).OrderBy(x => x.Title);
                            IOrderedEnumerable<Node> s = nodes.Where(x => x.NodeType == NodeType.Channel).OrderBy(x => x.Title);

                            category.Items.Clear();
                            category.Items.AddRange(c);
                            category.Items.AddRange(s);
                        }
                        else
                        {
                            IOrderedEnumerable<Node> c = nodes.Where(x => x.NodeType == NodeType.Category).OrderBy(x => x.Title);
                            IOrderedEnumerable<Node> s = nodes.Where(x => x.NodeType == NodeType.Channel).OrderByDescending(x => x.Channel.Date);

                            category.Items.Clear();
                            category.Items.AddRange(c);
                            category.Items.AddRange(s);
                        }
                    }
                    else
                    {
                        category.Items.Add(new Node());
                    }
                }
            }

            // update root
            _root.Count = Repository.Default.Videos.UnwatchedCount();
        }

        public void CreateCategory()
        {
            if (!Kernel.Default.Search)
            {
                Node parent = SelectedNodes.Cast<Node>().FirstOrDefault(x => x.NodeType == NodeType.Category) ?? _root;

                NodeTree.ScrollToNode(parent);

                Category category = new Category();
                category.Id = Identifier.New;
                category.Parent = parent.Id;
                category.Title = Properties.Resources.Title_NewCategory;

                Repository.Default.Categories.Insert(category);

                if (parent.IsExpanded)
                {
                    Node node = NodeFactory.CreateCategoryNode(category, parent);
                    node.Expanded += CategoryNode_Expanded;
                    node.Collapsed += CategoryNode_Collapsed;

                    parent.Items.Insert(0, node);

                    //NodeTree.Select(node);

                    node.IsRenaming = true;
                }
                else
                {
                    parent.IsExpanded = true;

                    Task.Delay(200).GetAwaiter().OnCompleted(() =>
                    {
                        Node node = _root.Find(category.Id);

                        if (node != null)
                        {
                            if (parent.Items.IndexOf(node) != 0)
                            {
                                parent.Items.Remove(node);
                                parent.Items.Insert(0, node);
                            }

                            //NodeTree.Select(node);
                            node.IsRenaming = true;
                        }
                    });
                }
            }
        }

        public void Rename()
        {
            Node node = SelectedNodes.Cast<Node>().FirstOrDefault();

            if (node != null)
            {
                node.IsRenaming = true;
            }
        }

        public async void Delete()
        {
            List<Node> selectedNodes = SelectedNodes.Cast<Node>().Where(x => x.CanDelete).ToList();

            if (selectedNodes.Count != 0)
            {
                List<string> channels_id = selectedNodes.Where(x => x.NodeType == NodeType.Channel).Select(x => x.Id).ToList();
                List<string> categories_id = selectedNodes.Where(x => x.NodeType == NodeType.Category).Select(x => x.Id).ToList();

                foreach (string category in categories_id.ToList())
                {
                    channels_id.AddRange(Repository.Default.Schema.GetChannelChildren(category));
                    categories_id.AddRange(Repository.Default.Schema.GetCategoryChildren(category));
                }

                List<string> ancestors = new List<string>();
                foreach (Node node in selectedNodes)
                {
                    foreach (string ancestor in Repository.Default.Schema.GetAncestors(node.Id))
                    {
                        ancestors.AddOrThrow(ancestor);
                    }
                }

                List<Channel> channels;

                if (channels_id.Count > 0)
                    channels = Repository.Default.Channels.Get().Where(x => channels_id.Contains(x.Id)).ToList();
                else
                    channels = new List<Channel>();

                bool allowDelete = true;

                if (channels.Count > 0)
                {
                    allowDelete = Dialogs.OpenDialog(Messages.GetDeleteMessage(channels));
                }

                if (allowDelete)
                {
                    Repository.Default.Categories.Remove(categories_id);
                    Repository.Default.Channels.Remove(channels_id);
                    Repository.Default.Videos.Remove(channels_id);

                    foreach (Node node in selectedNodes)
                    {
                        SelectedNodes.Remove(node);
                        node.Remove();
                    }

                    if (Kernel.Default.Search)
                    {
                        selectedNodes.ForEach(x => _root.Find(x.Id)?.Remove());
                    }

                    Move.Extract(selectedNodes);

                    if (Kernel.Default.Search)
                    {
                        foreach (string id in channels_id)
                        {
                            Node node = Nodes.FirstOrDefault(x => x.Id == id);

                            if (node != null)
                            {
                                Nodes.Remove(node);
                            }
                        }
                    }

                    if (channels_id.Count > 0)
                    {
                        _root.Count = Repository.Default.Videos.UnwatchedCount();

                        foreach (string ancestor in ancestors)
                        {
                            Node node = _root.Find(ancestor);

                            if (node != null && node.Equals(_root) == false)
                                node.Count = Repository.Default.Schema.GetCategoryState(ancestor);
                        }

                        ViewRepository.Get<FooterView>().UpdateCount();

                        ViewRepository.Get<VideoView>().RemoveAll();
                        Kernel.Default.IsDeleteing = true;
                        await RemoveSubscriptions.Execute(channels.Select(x => x.SubscriptionId));
                        Kernel.Default.IsDeleteing = false;
                    }

                    await Task.Run(() =>
                    {
                        if (File.Exists(FileSystem.Service))
                        {
                            try
                            {
                                int count = FileSystem.Service.ReadText().ToInt();

                                count -= channels_id.Count;

                                FileSystem.Service.WriteText(count.ToString());
                            }
                            catch (Exception e)
                            {
                                Logger.Log(MethodBase.GetCurrentMethod(), e);
                            }
                        }
                    });
                }
            }
        }

        public void Cut()
        {
            if (SelectedNodes.Count > 0)
            {
                List<Node> selectedNodes = SelectedNodes.Cast<Node>().Where(x => x.CanMove).ToList();

                if (selectedNodes.Count != 0)
                {
                    Move.Clear();

                    Move.Add(selectedNodes);

                    Kernel.Default.CanPaste = true;
                }
            }
        }

        public void Paste()
        {
            if (!Kernel.Default.Search && Kernel.Default.CanPaste)
            {
                Node newParent = SelectedNodes.First() as Node;
                if (newParent == null || newParent.NodeType == NodeType.Channel) return;

                Dictionary<string, string> channels = new Dictionary<string, string>();
                Dictionary<string, string> categories = new Dictionary<string, string>();

                List<string> parents = new List<string>();

                // kvp => key = id, value = type(0 = channel, 1 = category)

                foreach (KeyValuePair<string, int> kvp in Move.Nodes)
                {
                    Node node = _root.Find(kvp.Key);

                    if (node != null)
                    {
                        Node oldParent = node.Parent as Node;

                        if (oldParent == null
                            || oldParent.Equals(newParent) // ugyanarra a parent node-ra dobom
                            || newParent.Equals(node) // saját magára dobom
                            || newParent.NodeType == NodeType.Channel
                            || Repository.Default.Schema.IsDescendant(node.Id, newParent.Id)) continue;

                        if (node.NodeType == NodeType.Channel)
                        {
                            parents.AddOrThrow(newParent.Id);
                            parents.AddOrThrow(oldParent.Id);
                        }

                        node.Move(oldParent, newParent);

                        node.Fade = false;
                    }
                    else
                    {
                        Channel channel = Repository.Default.Channels.Get_By_Id(kvp.Key);
                        if (channel == null) continue;

                        node = NodeFactory.CreateChannelNode(channel, newParent);

                        // string oldParent = Repository.Default.Categories.Get_By_Id(kvp.Key)?.Id;

                        if (node == null // node valamiért null
                           || newParent.Id == channel.Parent  // az új parent megegyezik a régivel
                           || newParent.Id == channel.Id) continue; // saját magára másolom

                        parents.AddOrThrow(newParent.Id);
                        parents.AddOrThrow(channel.Parent);

                        newParent.Items.Add(node);
                        newParent.Sort();
                    }

                    // kvp.Value = type (channel/category)
                    if (kvp.Value == 0)
                    {
                        channels.Add(kvp.Key, newParent.Id);
                    }
                    else
                    {
                        categories.Add(kvp.Key, newParent.Id);
                    }
                }

                if (channels.Count > 0 || categories.Count > 0)
                {
                    Move.Clear();

                    Repository.Default.Channels.Move(channels);
                    Repository.Default.Categories.Move(categories);

                    UpdateParents(parents);

                    Kernel.Default.CanPaste = false;

                    newParent.Sort();

                    List<string> new_channels = new List<string>(Kernel.Default.SelectedChannels);
                    foreach (KeyValuePair<string, string> item in channels)
                    {
                        new_channels.Add(item.Key);
                    }

                    ViewRepository.Get<VideoView>().SelectedChannelChanged(new_channels);
                }
            }
        }

        public void Reset()
        {
            Search.Clear();
            Nodes.Clear();
            SelectedNodes.Clear();
            ClearMove();

            CreateRoot();
        }

        private void ClearMove()
        {
            Move.Clear();

            Kernel.Default.CanPaste = false;
        }

        public void DecreaseVideoCount(string id)
        {
            Node node = _root.Find(id);

            if (node != null)
            {
                node.Count--;

                if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.ActiveSubscriptions)
                {
                    if (node.Count == 0)
                    {
                        SelectNext(node);
                        VisibleNodes.Default.Remove(id);
                        node.Remove();
                    }
                }
            }

            List<string> ancestors = Repository.Default.Schema.GetAncestors(id);

            foreach (string ancestor in ancestors)
            {
                if (ancestor == id) continue;

                node = _root.Find(ancestor);

                if (node != null)
                    node.Count--;
            }

            _root.Count--;
        }

        private void SelectNext(Node node)
        {
            if (Kernel.Default.View == View.Subscriptions)
            {
                if (node.Parent is Node parent)
                {
                    int index = parent.Items.IndexOf(node);
                    index++;

                    if (parent.Items.Count - 1 >= index)
                    {
                        if (parent.Items[index] is Node nextChannel)
                        {
                            NodeTree.Select(nextChannel);
                        }
                    }
                    else
                    {
                        if (parent != _root)
                            NodeTree.Select(parent);
                    }
                }
            }
        }

        public void UpdateParents(List<string> ids)
        {
            foreach (string id in ids)
            {
                Node node = _root.Find(id);

                if (node != null && node.Id != Identifier.Empty)
                {
                    node.Count = Repository.Default.Categories.GetState(id);
                }
            }
        }

        public void CopyChannelPageUrls()
        {
            IEnumerable<Node> nodes = SelectedNodes.Cast<Node>();

            IEnumerable<Channel> channels = Repository.Default.Channels.Get();

            StringBuilder stringBuilder = new StringBuilder();

            foreach (Node node in nodes)
            {
                if (node.NodeType == NodeType.Channel)
                {
                    Channel channel = channels.FirstOrDefault(x => x.Id == node.Id);

                    if (channel != null)
                    {
                        stringBuilder.AppendLine($"{channel.Title} - {UrlHelper.GetChannelUrl(channel.Id)}");
                    }

                }
                else
                {
                    channels.Where(x => x.Parent == node.Id).ForEach(x =>
                    {
                        stringBuilder.AppendLine($"{x.Title} - {UrlHelper.GetChannelUrl(x.Id)}");
                    });
                }

                stringBuilder.AppendLine();
            }

            Clipboard.SetText(stringBuilder.ToString());
        }

        private void Locate()
        {
            if (_channelId != null)
            {
                Select(_channelId, _videoId);

                _channelId = null;
                _videoId = null;
            }
        }

        public void Locate(string channelId, string videoId)
        {
            if (ChannelAvailability.Check(channelId))
            {
                if (Kernel.Default.View != View.Subscriptions)
                    Kernel.Default.View = View.Subscriptions;

                Select(channelId, videoId);
            }
            else
            {
                if (Dialogs.OpenDialog(Messages.ChangeSubscriptionView()))
                {
                    ChangeAndSelect(channelId, videoId);
                }
            }
        }

        private async void Select(string channelId, string videoId = null)
        {
            Search.Clear();

            List<string> ancestors = Repository.Default.Schema.GetAncestors(channelId);

            foreach (string ancestor in ancestors)
            {
                if (ancestor == ancestors.Last())
                {
                    Node node = _root.Find(ancestor);
                    if (node != null)
                    {
                        bool select = !node.IsSelected;

                        NodeTree.ScrollToNode(node, select);
                    }
                }
                else
                {
                    if (ancestor == Identifier.Empty)
                    {
                        _root.IsExpanded = true;
                    }
                    else
                    {
                        Node node = _root.Find(ancestor);
                        if (node != null)
                            node.IsExpanded = true;
                    }
                }

                await Task.Delay(100);
            }

            if (videoId != null)
            {
                ViewRepository.Get<VideoView>().Locate(videoId);
            }
        }

        private void ChangeAndSelect(string channelId, string videoId)
        {
            _channelId = channelId;
            _videoId = videoId;

            Settings.Default.SubscriptionDisplayMode = SubscriptionDisplayMode.AllSubscriptions;

            Search.Clear();
        }

        public void ChangeColor()
        {
            Node node = SelectedNodes.Cast<Node>().FirstOrDefault(x => x.NodeType == NodeType.Category);

            if (node != null)
            {
                ColorPicker colorPicker = new ColorPicker(node);
                colorPicker.Owner = Application.Current.MainWindow;
                colorPicker.ShowDialog();
            }
        }

        public void DefaultColor()
        {
            Node node = SelectedNodes.Cast<Node>().FirstOrDefault(x => x.NodeType == NodeType.Category);

            if (node != null)
            {
                node.Color = ColorHelper.GetDefaultFolderColor();

                node.Category.Color = null;

                Repository.Default.Categories.Update(node.Category);
            }
        }

        public void MarkUnwatched()
        {
            if (SelectedNodes.Count == 0 || SelectedNodes.Contains(_root))
            {
                MarkAllUnwatched();
            }
            else
            {
                try
                {
                    Mark mark = new Mark();
                    mark.MarkUnwached(SelectedNodes, _root);
                }
                catch (Exception ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = GetType().Assembly.FullName,
                        ClassName = GetType().FullName,
                        MethodName = MethodBase.GetCurrentMethod().Name,
                        ExceptionType = ex.GetType().ToString(),
                        Message = ex.Message,
                        Trace = ex.StackTrace,
                    };

                    Logger.Log(error);

                    #endregion
                }
            }
        }

        public void MarkAllUnwatched()
        {
            try
            {
                Mark mark = new Mark();
                mark.MarkAllUnwached(SelectedNodes, _root);
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = GetType().Assembly.FullName,
                    ClassName = GetType().FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }
        }

        public void MarkWatched()
        {
            if (SelectedNodes.Count == 0 || SelectedNodes.Contains(_root))
            {
                MarkAllWatched();
            }
            else
            {
                try
                {
                    Mark mark = new Mark();
                    mark.MarkWached(SelectedNodes, _root);
                }
                catch (Exception ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = GetType().Assembly.FullName,
                        ClassName = GetType().FullName,
                        MethodName = MethodBase.GetCurrentMethod().Name,
                        ExceptionType = ex.GetType().ToString(),
                        Message = ex.Message,
                        Trace = ex.StackTrace,
                    };

                    Logger.Log(error);

                    #endregion
                }
            }
        }

        public void MarkAllWatched(bool showDialog = true)
        {
            void Func()
            {
                try
                {
                    Mark mark = new Mark();
                    mark.MarkAllWached(_root);
                }
                catch (Exception ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = GetType().Assembly.FullName,
                        ClassName = GetType().FullName,
                        MethodName = MethodBase.GetCurrentMethod().Name,
                        ExceptionType = ex.GetType().ToString(),
                        Message = ex.Message,
                        Trace = ex.StackTrace,
                    };

                    Logger.Log(error);

                    #endregion
                }
            }

            if (showDialog)
            {
                if (Settings.Default.MarkAllWatchedDialog)
                {
                    if (Dialogs.OpenDialog(Properties.Resources.Question_MarkAllVideoWatched))
                    {
                        Func();
                    }
                }
                else
                {
                    Func();
                }
            }
            else
            {
                Func();
            }
        }

        public void Logout()
        {
            SearchBox.Text = null;
            SearchBox.IsEnabled = false;

            Nodes.Clear();
            SelectedNodes.Clear();
            _root = null;

            NodeTree.ContextMenu = null;
        }

        public void Login()
        {
            CreateRoot();
            SearchBox.IsEnabled = true;
            SetContextMenu();
        }
    }
}
