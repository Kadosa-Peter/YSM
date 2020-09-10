using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ysm.Core
{
    public static partial class ExtensionMethods
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Visibility ToReveseVisibility(this bool boolean)
        {
            if (boolean)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public static T GetParentOfType<T>(this DependencyObject @this) where T : DependencyObject
        {
            if (@this == null) return null;

            DependencyObject parentObject = VisualTreeHelper.GetParent(@this);

            if (parentObject == null) return null;

            if (parentObject is T parent)
            {
                return parent;
            }

            return GetParentOfType<T>(parentObject);
        }

        public static T GetChildOfType<T>(this DependencyObject @this) where T : DependencyObject
        {
            if (@this == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(@this); i++)
            {
                var child = VisualTreeHelper.GetChild(@this, i);

                var result = child as T ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        public static T GetChildOfType<T>(this DependencyObject @this, string name) where T : DependencyObject
        {
            if (@this == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(@this);

            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(@this, i);

                if (!(child is T))
                {
                    foundChild = GetChildOfType<T>(child, name);

                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(name))
                {
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == name)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static void ForceFocus(this Control element)
        {
            if (element == null) return;

            if (!element.Focus())
            {
                element.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() => element.Focus()));
            }
        }

        public static void AutoScroll(this ScrollViewer scrollViewer, DragEventArgs e)
        {
            Point position = e.GetPosition(scrollViewer);

            double height = scrollViewer.ActualHeight;

            if (position.Y <= 10)
            {
                double offSet = scrollViewer.VerticalOffset - 10;

                scrollViewer.ScrollToVerticalOffset(offSet);
            }

            if (position.Y >= height - 10)
            {
                double offSet = scrollViewer.VerticalOffset + 10;

                scrollViewer.ScrollToVerticalOffset(offSet);
            }
        }

        public static void SetVerticalOffset(this ListView listView, double value)
        {
            if (value > 0)
            {
                ScrollViewer scrollViewer = listView.GetChildOfType<ScrollViewer>();

                if (listView != null)
                {
                    if (scrollViewer.VerticalOffset > 0)
                    {
                        double offset = scrollViewer.VerticalOffset + value;
                        scrollViewer.ScrollToVerticalOffset(offset);
                    }
                }
            }
        }

        public static double GetScrollableHeight(this ListView listView)
        {
            ScrollViewer scrollViewer = listView.GetChildOfType<ScrollViewer>();

            if (listView != null)
            {
                scrollViewer.UpdateLayout();

                return scrollViewer.ScrollableHeight;
            }

            return -1;
        }

      
    }
}
