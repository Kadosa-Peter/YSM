using System.Windows;
using System.Windows.Controls;

namespace Ysm.Controls
{
    public class ExtendedComboBoxItem : ComboBoxItem
    {
        #region IsChecked

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
            "IsChecked", typeof(bool), typeof(ExtendedComboBoxItem), new PropertyMetadata(default(bool)));

        public bool IsChecked
        {
            get => (bool) GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        #endregion

        static ExtendedComboBoxItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedComboBoxItem), new FrameworkPropertyMetadata(typeof(ExtendedComboBoxItem)));
        }
    }
}
