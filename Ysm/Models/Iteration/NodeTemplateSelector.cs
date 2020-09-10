using System.Windows;
using System.Windows.Controls;

namespace Ysm.Models.Iteration
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
                case NodeType.Date:
                    templateName = "DateTemplate";
                    break;
                case NodeType.Iteration:
                    templateName = "IterationTemplate";
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
