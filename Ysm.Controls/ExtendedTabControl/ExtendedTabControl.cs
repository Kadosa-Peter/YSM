using System.Windows;
using System.Windows.Controls;

namespace Ysm.Controls
{
    public class ExtendedTabControl : TabControl
    {
        private ScrollViewer _scrollViewer;

        static ExtendedTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedTabControl), new FrameworkPropertyMetadata(typeof(ExtendedTabControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
            if (_scrollViewer != null) _scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (_scrollViewer == null) return;

            if (e.Delta < 0)
                _scrollViewer.LineRight();
            else
                _scrollViewer.LineLeft();

            e.Handled = true;
        }

        public int Count => Items.Count;

        public void Remove(object obj)
        {
            Items.Remove(obj);
        }

        public void Add(object obj)
        {
            Items.Add(obj);
        }

        public void Clear()
        {
            Items.Clear();
        }


    }
}
