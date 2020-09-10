using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Ysm.Controls
{
    public class ExtendedListBox : ListBox
    {
        #region SelectedItems

        public new static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems", typeof (ObservableCollection<object>), typeof (ExtendedListBox));

        public new ObservableCollection<object> SelectedItems
        {
            get => (ObservableCollection<object>) GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        #endregion

        #region SelectedItemsChanged

        public static readonly RoutedEvent SelectedItemsChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedItemsChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedListBox));

        public event RoutedEventHandler SelectedItemsChanged
        {
            add => AddHandler(SelectedItemsChangedEvent, value);
            remove => RemoveHandler(SelectedItemsChangedEvent, value);
        }

        private void RaiseSelectedItemsChangedEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(SelectedItemsChangedEvent);
            RaiseEvent(args);
        }
        
        #endregion

        static ExtendedListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedListBox), new FrameworkPropertyMetadata(typeof(ExtendedListBox)));

            SelectionModeProperty.OverrideMetadata(typeof(ExtendedListBox), new FrameworkPropertyMetadata(SelectionMode.Extended));
        }

        public ExtendedListBox()
        {
            SelectedItems = new ObservableCollection<object>();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (SelectionMode == SelectionMode.Single)
            {
                if (e.AddedItems.Count <= 0) return;

                SelectedItems.Clear();

                foreach (object item in e.AddedItems)
                {
                    SelectedItems.Add(item);
                }

                RaiseSelectedItemsChangedEvent();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #region Container

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ExtendedListBoxItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ExtendedListBoxItem;
        }

        #endregion
    }
}
