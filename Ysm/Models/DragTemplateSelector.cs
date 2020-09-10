using System.Windows;
using System.Windows.Controls;

namespace Ysm.Models
{
    public class DragTemplateSelector : DataTemplateSelector
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
                    templateName = "DragChannelTemplate";
                    break;
                case NodeType.Category:
                    templateName = "DragCategoryTemplate";
                    break;
                default:
                    return null;
            }

            DataTemplate template = element.FindResource(templateName) as DataTemplate;
            return template;
        }
    }
}
