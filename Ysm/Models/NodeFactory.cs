using System.Collections.Generic;
using System.Linq;
using Ysm.Assets;
using Ysm.Core;
using Ysm.Data;
using Ysm.Views;

namespace Ysm.Models
{
    public static class NodeFactory
    {
        public static Node CreateRoot()
        {
            Category category = new Category();
            category.Id = Identifier.Empty;
            category.Title = Properties.Resources.Title_Subscriptions;

            Node node = CreateCategoryNode(category, null);
            node.Items.Clear();

            node.CanDelete = false;
            node.CanRename = false;
            node.CanMove = false;
            node.CanCollapse = false;
            node.IsExpanded = true;

            node.NodeType = NodeType.Root;

            node.Count = Repository.Default.Videos.UnwatchedCount();

            Move.Root = node;

            return node;
        }

        public static Node CreateChannelNode(Channel channel, Node parent, int channelState = -1)
        {
            Node node = new Node();
            node.Channel = channel;
            node.NodeType = NodeType.Channel;
            node.Parent = parent;
            node.CanCollapse = true;
            node.CanRename = false;
            node.Title = channel.Title;

            if (Move.Contains(channel.Id))
                node.Fade = true;

            node.Icon = UrlHelper.GetIcon(channel.Id, 22, 22);

            if (channelState == -1)
                node.Count = Repository.Default.Channels.GetState(channel.Id);
            else
                node.Count = channelState;

            return node;
        }

        public static Node CreateCategoryNode(Category category, Node parent)
        {
            Node node = new Node();
            node.Category = category;
            node.NodeType = NodeType.Category;
            node.Parent = parent;
            node.CanCollapse = true;
            node.Title = category.Title;
            //node.RenameEnded += (s, e) => { node.Title = category.Title; };
            node.RenameEnded += Node_RenameEnded;
            node.RenameStarted += Node_RenameStarted;

            if (category.Color.NotNull())
            {
                node.Color= category.Color.ToColor();
            }

            if (Move.Contains(category.Id))
                node.Fade = true;

            node.Count = Repository.Default.Categories.GetState(category.Id);

            if (Settings.Default.SubscriptionDisplayMode == SubscriptionDisplayMode.ActiveSubscriptions)
            {
                if (Repository.Default.Categories.HasUnwatchedVideoOrCategory(category.Id))
                {
                    node.Items.Add(new Node());
                }
            }
            else
            {
                if (Repository.Default.Categories.HasChildren(category.Id))
                {
                    node.Items.Add(new Node());
                }
            }

            return node;
        }

        private static void Node_RenameStarted(object sender, System.EventArgs e)
        {
            Kernel.Default.IsRenaming = true;
        }

        private static void Node_RenameEnded(object sender, System.EventArgs e)
        {
            if (sender is Node node)
            {
                if (node.Title != node.Category.Title)
                {
                    Repository.Default.Categories.Update(node.Category);

                    node.Title = node.Category.Title;

                    if (node.Parent is Node parent)
                    {
                        // nem lehet 2x átnevezni egymás után, a hiba valahol itt van
                        List<Node> categoryNodes = parent.Items.Cast<Node>().Where(x => x.NodeType == NodeType.Category).OrderBy(x => x.Title).ToList();
                        List<Node> channelNodes = parent.Items.Cast<Node>().Where(x => x.NodeType == NodeType.Channel).ToList();

                        parent.Items.Clear();

                        parent.Items.AddRange(categoryNodes);
                        parent.Items.AddRange(channelNodes);

                        // Hack: új mappa létrehozásakor a root IsRenaming tulajdonsága true lessz!
                        node.Parent.IsRenaming = false;

                        // Hack: valamiért átnevezés után a NodeTree elveszti a fokuszt, így nem működik tovább a KeyDown event
                        ViewRepository.Get<ChannelView>().NodeTree.ForceFocus();
                    }
                }
            }

            Kernel.Default.IsRenaming = false;
        }
    }
}
