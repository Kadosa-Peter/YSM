using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MoreLinq;
using Ysm.Assets;
using Ysm.Assets.Menu;
using Ysm.Core;
using Ysm.Data;
using Ysm.Models.Notes;
using Ysm.Windows;

namespace Ysm.Views
{
    public partial class MarkerView
    {
        public ObservableCollection<Node> Nodes { get; set; }

        public ObservableCollection<object> SelectedNodes { get; set; }

        public MarkerView()
        {
            InitializeComponent();

            ViewRepository.Add(this, nameof(MarkerView));

            Nodes = new ObservableCollection<Node>();
            SelectedNodes = new ObservableCollection<object>();

            if (AuthenticationService.Default.IsLoggedIn)
            {
                MarkerMenu menu = new MarkerMenu();
                NodeTree.ContextMenu = menu.Get();
            }
        }

        private void MarkerView_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadNodes();
        }

        private void NodeTree_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Video video = GetSelectedVideo();

            if (video != null)
            {
                if (ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.Video.VideoId == video.VideoId)
                {
                    ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.SeekTo(video.Start);
                }
                else
                {
                    ViewRepository.Get<PlayerTabView>().Open(video);
                }
            }
        }

        private void NodeTree_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SearchBox.Clear();
            }
            if (e.Key == Key.F2)
            {
                Edit();
            }
            else if (e.Key == Key.Delete)
            {
                Remove();
            }
        }

        private void LoadNodes()
        {
            Nodes.Clear();
            SelectedNodes.Clear();

            List<MarkerGroup> markerGroups = Repository.Default.Markers.Get();

            foreach (MarkerGroup markerGroup in markerGroups.OrderBy(x => x.Title))
            {
                Node node = CreateMarkerGroup(markerGroup);
                Nodes.Add(node);
            }
        }

        public void AddMarker(MarkerGroup markerGroup, Marker entry)
        {
            Node parent = Nodes.FirstOrDefault(x => x.Id == markerGroup.Id);

            if (parent != null)
            {
                parent.MarkerGroup.Markers.Add(entry);

                if (parent.IsExpanded)
                {
                    parent.Items.Clear();

                    foreach (Marker marker in parent.MarkerGroup.Markers.OrderBy(x => x.Time))
                    {
                        Node node = new Node();
                        node.Marker = marker;
                        node.Parent = parent;
                        node.NodeType = NodeType.Marker;
                        parent.Items.Add(node);
                    }
                }
            }
            else
            {
                Node node = CreateMarkerGroup(markerGroup);
                Nodes.Add(node);
                List<Node> sortedNodes = Nodes.OrderBy(x => x.MarkerGroup.Title).ToList();
                Nodes.Clear();
                Nodes.AddRange(sortedNodes);
            }
        }



        private Node CreateMarkerGroup(MarkerGroup markerGroup)
        {
            Node node = new Node();
            node.NodeType = NodeType.Group;
            node.MarkerGroup = markerGroup;
            node.Icon = UrlHelper.GetIcon(markerGroup.ChannelId, 22, 22);

            // TODO: should be true by default
            node.CanCollapse = true;

            node.Expanded += Node_Expanded;

            node.Items.Add(new Node());

            return node;
        }

        private void Node_Expanded(object sender, System.EventArgs e)
        {
            if (sender is Node node)
            {
                node.Items.Clear();

                CreateEntries(node);
            }
        }

        private static void CreateEntries(Node parent)
        {
            foreach (Marker entry in parent.MarkerGroup.Markers.OrderBy(x => x.Time))
            {
                Node node = new Node();
                node.Marker = entry;
                node.Parent = parent;
                node.NodeType = NodeType.Marker;

                parent.Items.Add(node);
            }
        }

        public bool CanEdit()
        {
            return SelectedNodes.Cast<Node>().Any(x => x.NodeType == NodeType.Marker);
        }

        public void Edit()
        {
            Node nodes = SelectedNodes.Cast<Node>().FirstOrDefault(x => x.NodeType == NodeType.Marker);

            if (nodes != null)
            {
                if (nodes.Parent is Node groupNode)
                {
                    MarkerUpdate window = new MarkerUpdate(groupNode.Title, groupNode.Id, nodes.Id, nodes.Marker.Comment, (int)nodes.Marker.Time.TotalSeconds);
                    window.Owner = Application.Current.MainWindow;
                    window.ShowDialog();
                }
            }
        }

        public bool CanRemove()
        {
            return SelectedNodes.Count > 0;
        }

        public void Remove()
        {
            if (SelectedNodes.Count > 0)
            {
                Node node = SelectedNodes.Cast<Node>().FirstOrDefault();

                if (node != null)
                {
                    if (node.NodeType == NodeType.Group)
                    {
                        RemoveGroup(node.MarkerGroup.Id);
                    }
                    else if (node.NodeType == NodeType.Marker)
                    {
                        removeMarker(node.Marker.GroupId, node.Marker.Id);
                    }
                }
            }
        }

        public void RemoveGroup(string id)
        {
            Node node = Nodes.FirstOrDefault(x => x.Id == id);

            if (node != null)
            {
                Nodes.Remove(node);

                List<Node> childNodes = SelectedNodes.Cast<Node>().Where(x => x.Parent == node).ToList();
                childNodes.ForEach(x => SelectedNodes.Remove(x));

                SelectedNodes.Remove(node);

                Repository.Default.Markers.Delete(id);

                ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView?.RemoveMarkers(id);
            }
        }

        public void removeMarker(string groupId, string markerId)
        {
            Node groupNode = Nodes.FirstOrDefault(x => x.Id == groupId);

            if(groupNode == null) return;

            if (groupNode.MarkerGroup.Markers.Count > 1)
            {
                groupNode.MarkerGroup.Markers.RemoveAll(x => x.Id == markerId);

                if (groupNode.IsExpanded)
                {
                    Node markerNode =groupNode.Items.Cast<Node>().FirstOrDefault(x => x.Id == markerId);
                    if (markerNode != null) groupNode.Items.Remove(markerNode);
                }
            }
            else
            {
                Nodes.Remove(groupNode);
            }

            ViewRepository.Get<PlayerTabView>()?.SelectedPlayerView.RemoveMarker(markerId);
        }

        public void UpdateMarker(string groupId, string markerId, string comment)
        {
            Node group = Nodes.FirstOrDefault(x => x.Id == groupId);

            if (group != null)
            {
                Marker marker = group.MarkerGroup.Markers.FirstOrDefault(x => x.Id == markerId);
                if (marker != null) marker.Comment = comment;

                if (group.IsExpanded)
                {
                    Node markerNode = group.Items.Cast<Node>().FirstOrDefault(x => x.Id == markerId);

                    if (markerNode != null)
                        markerNode.Marker.Comment = comment;
                }
            }


        }

        public void Reset()
        {
            LoadNodes();
        }

        public void Login()
        {
            MarkerMenu menu = new MarkerMenu();
            NodeTree.ContextMenu = menu.Get();

            if (Kernel.Default.View == View.Markers)
            {
                LoadNodes();
            }
        }

        public void Logout()
        {
            SearchBox.Text = null;
            SearchBox.IsEnabled = false;

            Nodes.Clear();
            SelectedNodes.Clear();

            NodeTree.ContextMenu = null;
        }

        public bool CanOpen()
        {
            return SelectedNodes.Count > 0;
        }

        public void Open()
        {
            Video video = GetSelectedVideo();

            if (video != null)
            {
                if (ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.Video.VideoId == video.VideoId)
                {
                    ViewRepository.Get<PlayerTabView>().SelectedPlayerView?.SeekTo(video.Start);
                }
                else
                {
                    ViewRepository.Get<PlayerTabView>().Open(video);
                }
            }
        }

        public void OpenTab()
        {
            Video video = GetSelectedVideo();

            if (video != null)
                ViewRepository.Get<PlayerTabView>()?.OpenTab(video);
        }

        private Video GetSelectedVideo()
        {
            Node node = SelectedNodes.Cast<Node>().FirstOrDefault();

            if (node != null)
            {
                MarkerGroup markerGroup;
                int seconds;

                if (node.NodeType == NodeType.Group)
                {
                    markerGroup = node.MarkerGroup;
                    seconds = 0;
                }
                else
                {
                    markerGroup = (node.Parent as Node)?.MarkerGroup;
                    seconds = (int)node.Marker.Time.TotalSeconds;
                }

                if (markerGroup != null)
                {
                    Video video = new Video();
                    video.Title = markerGroup.Title;
                    video.VideoId = markerGroup.Id;
                    video.ChannelId = markerGroup.ChannelId;
                    video.Duration = markerGroup.Duration;
                    video.Link = markerGroup.Link;
                    video.Published = markerGroup.Published.DateTime;
                    video.ThumbnailUrl = markerGroup.ThumbnailUrl;
                    video.Start = seconds;

                    return video;
                }
            }

            return null;
        }

        public string SearchQuery
        {
            set
            {
                if (value.IsNull())
                {
                    Nodes.ForEach(x => x.Visibility = Visibility.Visible);
                }
                else
                {
                    string search = value.ToLower();
                    foreach (Node node in Nodes)
                        node.Visibility = node.MarkerGroup.Title.Contains(search, true) ? Visibility.Visible : Visibility.Collapsed;

                }
            }
        }


    }
}
