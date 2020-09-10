using System.Windows;
using System.Windows.Controls;

namespace Ysm.Models
{
    public class NodeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Node node = item as Node;

            FrameworkElement element = container as FrameworkElement;

            if (node == null || element == null) return null;

            string templateName;

            switch (node.NodeType)
            {
                case NodeType.Channel:
                    templateName = "ChannelTemplate";
                    break;
                case NodeType.Category:
                    templateName = "CategoryTemplate";
                    break;
                case NodeType.Playlist:
                    templateName = "PlaylistTemplate";
                    break;
                case NodeType.Bookmark:
                    templateName = "BookmarkTemplate";
                    break;
                case NodeType.Root:
                    templateName = "RootCategoryTemplate";
                    break;
                default:
                    templateName = string.Empty;
                    break;
            }

            DataTemplate template = element.FindResource(templateName) as DataTemplate;
            return template;
        }
    }
}
