using System.Windows;
using System.Windows.Controls;

namespace Ysm.Models.Notes
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
                case NodeType.Root:
                    templateName = "RootTemplate";
                    break;
                case NodeType.Group:
                    templateName = "MarkerGroupTemplate";
                    break;
                case NodeType.Marker:
                    templateName = "MarkerTemplate";
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
