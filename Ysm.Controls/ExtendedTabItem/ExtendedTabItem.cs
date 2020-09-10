using System.Windows;
using System.Windows.Controls;

namespace Ysm.Controls
{
    public class ExtendedTabItem : TabItem
    {
        #region Close Event

        public static readonly RoutedEvent CloseEvent = EventManager.RegisterRoutedEvent(
            "Close", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedTabItem));

        public event RoutedEventHandler Close
        {
            add => AddHandler(CloseEvent, value);
            remove => RemoveHandler(CloseEvent, value);
        }

        private void RaiseCloseEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(CloseEvent);
            RaiseEvent(args);
        }

        #endregion

        static ExtendedTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedTabItem), new FrameworkPropertyMetadata(typeof(ExtendedTabItem)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_CloseButton") is Button button) button.Click += CloseButton_OnClick;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            RaiseCloseEvent();
        }
    }
}
