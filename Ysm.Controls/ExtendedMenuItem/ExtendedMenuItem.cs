using System.Windows;
using System.Windows.Controls;

namespace Ysm.Controls
{
    public class ExtendedMenuItem : MenuItem
    {
        #region Hotkey
        public static readonly DependencyProperty HotkeyProperty = DependencyProperty.Register(
            "Hotkey", typeof(string), typeof(ExtendedMenuItem), new PropertyMetadata(default(string)));

        public string Hotkey
        {
            get => (string)GetValue(HotkeyProperty);
            set => SetValue(HotkeyProperty, value);
        }
        #endregion

        #region IsSeparator

        public static readonly DependencyProperty IsSeparatorProperty = DependencyProperty.Register(
            "IsSeparator", typeof(bool), typeof(ExtendedMenuItem), new PropertyMetadata(default(bool)));

        public bool IsSeparator
        {
            get => (bool)GetValue(IsSeparatorProperty);
            set => SetValue(IsSeparatorProperty, value);
        }

        #endregion

        static ExtendedMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedMenuItem), new FrameworkPropertyMetadata(typeof(ExtendedMenuItem)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Loaded += (s, e) =>
            {
                if (!IsSeparator) return;

                Style style = FindResource("MenuSeparatorStyle") as Style;
                Style = style;

                Focusable = false;
                IsHitTestVisible = false;
            };
        }
    }
}
