using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using Ysm.Controls;
using Ysm.Core;
using Ysm.Data;
using Ysm.Models;

namespace Ysm.Assets.Behaviors
{
    public class VideoDropBehvior : Behavior<ExtendedTreeView>
    {
        private ExtendedTreeView _treeView;

        protected override void OnAttached()
        {
            _treeView = AssociatedObject;

            _treeView.AllowDrop = true;

            _treeView.Drop += TreeView_Drop;
        }

        private void TreeView_Drop(object sender, DragEventArgs e)
        {
            Node node = GetDropTarget(e);

            if (node != null)
            {
                Video video = (Video)e.Data.GetData("YSM-Video");

                if (video == null) return;

                if (node.Parent is Node root) root.Count++;

                if (node.Playlist.Videos.Any(x => x.VideoId == video.VideoId))
                {
                    string message = Properties.Resources.Title_PlaylistContainsVideo;
                    message = message.Replace("_XY_", node.Playlist.Name);

                    Dialogs.OpenInfo(Messages.PlaylistContainsVideo(message, video.Title));
                }
                else
                {
                    node.Playlist.Add(video);
                    node.Count++;

                    Node channelNode = node.Items.Cast<Node>().FirstOrDefault(x => x.Id == video.ChannelId);

                    if (channelNode == null)
                    {
                        Channel channel = Repository.Default.Channels.Get_By_Id(video.ChannelId);

                        channelNode = CreateChannelNode(channel, node);

                        node.Items.Add(channelNode);
                    }
                    else
                    {
                        channelNode.Count++;
                    }

                    Repository.Default.Playlists.Save(node.Playlist);
                }
            }

            ExtendedTreeItem item = (e.OriginalSource as DependencyObject).GetParentOfType<ExtendedTreeItem>();
            if (item != null) item.IsDragOver = false;
        }

        private Node GetDropTarget(DragEventArgs e)
        {
            ExtendedTreeItem item = (e.OriginalSource as DependencyObject).GetParentOfType<ExtendedTreeItem>();

            if (item == null) return null;

            Node node = item.Header as Node;

            if (node == null) return null;

            if (node.Playlist != null)
                return node;

            if (node.Parent is Node parentNode)
            {
                if (parentNode.Playlist != null)
                    return parentNode;
            }

            return null;
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
    }
}
