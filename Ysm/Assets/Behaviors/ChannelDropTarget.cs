using System.Windows;
using Ysm.Controls;
using Ysm.Core;
using Ysm.Models;

namespace Ysm.Assets.Behaviors
{
    public class ChannelDropTarget
    {
        public Node Node { get; }

        public Node Source { get; }

        public ChannelDropTarget(DragEventArgs e)
        {
            ExtendedTreeItem item = (e.OriginalSource as DependencyObject).GetParentOfType<ExtendedTreeItem>();

            if (item != null)
            {
                if (item.Header is Node node)
                {
                    Source = node;

                    if (node.NodeType == NodeType.Channel)
                    {
                        Node = node.Parent as Node;
                    }
                    else
                    {
                        Node = node;
                    }
                }
            }
            else
            {
                ExtendedTreeView treeView = (ExtendedTreeView)e.Source;

                ExtendedTreeItem extendedTreeItem = treeView.Items[0] as ExtendedTreeItem;

                if (extendedTreeItem?.Header is Node root)
                {
                    Node = root;

                    Source = root;
                }
            }
        }
    }

}
