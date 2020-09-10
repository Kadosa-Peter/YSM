using System.Windows;
using System.Windows.Controls;

namespace Ysm.Controls
{
    public class ExtendedListBoxItem : ListBoxItem
    {
        #region IsLoaded

        public static readonly DependencyProperty IsLoadedProperty = DependencyProperty.Register(
            "IsLoaded", typeof(bool), typeof(ExtendedListBoxItem), new PropertyMetadata(default(bool)));

        public new bool IsLoaded
        {
            get => (bool)GetValue(IsLoadedProperty);
            set => SetValue(IsLoadedProperty, value);
        }

        #endregion

        public ExtendedListBoxItem()
        {
            RequestBringIntoView += (s, e) => { e.Handled = true; };

            Loaded += (s, e) => { IsLoaded = true; };
        }
    }
}
