using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ysm.Assets;
using Ysm.Core;

namespace Ysm.Models
{
    public static class NodeExtensions
    {
        public static List<Node> GetChildren(this Node parent)
        {
            List<Node> children = new List<Node>();

            foreach (NodeBase nodeBase in parent.Items)
            {
                Node node = (Node)nodeBase;

                if (node == null) continue;

                children.Add(node);

                if (node.IsExpanded)
                {
                    children.AddRange(node.GetChildren());
                }
            }

            return children;
        }

        public static void Move(this Node node, Node oldParent, Node newParent)
        {
            // remove from old parent
            if (oldParent.Items.Contains(node))
            {
                oldParent.Items.Remove(node);

                if (oldParent.Items.Count == 0)
                {
                    oldParent.IsExpanded = false;
                    oldParent.HasItems = false;
                }
            }

            // add to new parent
            if (newParent.IsExpanded)
            {
                node.Parent = newParent;

                if (node.NodeType == NodeType.Category)
                {
                    node.Category.Parent = newParent.Id;
                }
                else
                {
                    node.Channel.Parent = newParent.Id;
                }

                newParent.Items.Add(node);
            }
            else
            {
                if (!newParent.HasItems)
                {
                    newParent.Items.Add(new Node());
                }
            }
        }

        public static Node Find(this Node node, string id)
        {
            if (id == node.Id)
                return node;

            foreach (NodeBase nodeBase in node.Items)
            {
                Node child = (Node)nodeBase;

                if (child.Id == id)
                {
                    return child;
                }

                if (child.HasItems)
                {
                    Node n = child.Find(id);

                    if (n != null)
                    {
                        return n;
                    }
                }
            }

            return null;
        }

        public static void Remove(this Node node)
        {
            if (node.Parent is Node parent)
            {
                parent.Items.Remove(node);

                if (parent.Items.Count == 0)
                {
                    parent.IsExpanded = false;
                    parent.HasItems = false;
                }
            }
        }

        public static List<Node> GetCategories(this Node @this, bool root = true)
        {
            List<Node> nodes = new List<Node>();

            if (@this.Id == Identifier.Empty && root == false)
            {
                // do not add root node
            }
            else
            {
                nodes.Add(@this);
            }

            foreach (NodeBase nodeBase in @this.Items)
            {
                Node children = (Node)nodeBase;

                if (children.NodeType == NodeType.Category)
                {
                    nodes.AddRange(children.GetCategories());
                }
            }

            return nodes;
        }

        public static List<Node> GetExpandedNodes(this Node @this)
        {
            List<Node> nodes = new List<Node>();

            foreach (Node node in @this.Items)
            {
                if (node.NodeType == NodeType.Category && node.IsExpanded)
                {
                    nodes.Add(node);

                    nodes.AddRange(node.GetExpandedNodes());
                }
            }

            return nodes;
        }

        public static List<Node> GetVisibleNodes(this Node @this)
        {
            List<Node> nodes = new List<Node>();

            foreach (NodeBase nodeBase in @this.Items)
            {
                Node node = (Node)nodeBase;
                nodes.Add(node);

                if (node.IsExpanded)
                {
                    nodes.AddRange(node.GetVisibleNodes());
                }
            }

            return nodes;
        }

        public static void Unselect(this ObservableCollection<object> nodes)
        {
            foreach (Node node in nodes)
            {
                node.IsSelected = false;
            }

            nodes.Clear();
        }

        public static List<Node> GetAncestors(this Node node)
        {
            List<Node> ancestors = new List<Node>();

            if (node.Parent is Node parent)
            {
                ancestors.Add(parent);

                ancestors.AddRange(parent.GetAncestors());
            }

            return ancestors;
        }

        public static void Sort(this Node parent)
        {
            if (parent != null && parent.IsExpanded)
            {
                List<Node> categoryNodes = parent.Items.
                    Cast<Node>().
                    Where(x => x.NodeType == NodeType.Category).
                    OrderBy(x => x.Category.Title).ToList();

                List<Node> channelNodes;

                if (Settings.Default.SubscriptionSort == SortType.Title)
                {
                    channelNodes = parent.Items.Cast<Node>().
                        Where(x => x.NodeType == NodeType.Channel).
                        OrderBy(x => x.Channel.Title).
                        ToList();
                }
                else
                {
                    channelNodes = parent.Items.Cast<Node>().
                        Where(x => x.NodeType == NodeType.Channel).
                        OrderByDescending(x => x.Channel.Date).
                        ToList();
                }

                parent.Items.Clear();

                parent.Items.AddRange(categoryNodes);
                parent.Items.AddRange(channelNodes);
            }
        }
    }
}
