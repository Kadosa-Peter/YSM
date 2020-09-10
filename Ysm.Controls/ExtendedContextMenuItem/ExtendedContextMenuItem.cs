using System.Windows;
using System.Windows.Controls;

namespace Ysm.Controls
{
    public class ExtendedContextMenuItem : MenuItem
    {
        #region Hotkey

        public static readonly DependencyProperty HotkeyProperty = DependencyProperty.Register(
            "Hotkey", typeof (string), typeof (ExtendedContextMenuItem), new PropertyMetadata(default(string)));

        public string Hotkey
        {
            get => (string) GetValue(HotkeyProperty);
            set => SetValue(HotkeyProperty, value);
        }

        #endregion

        #region Identifier

        public static readonly DependencyProperty IdProperty = DependencyProperty.Register(
            "Identifier", typeof (string), typeof (ExtendedContextMenuItem), new PropertyMetadata(default(string)));

        public string Id
        {
            get => (string) GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        #endregion

        #region IsSeparator

        public static readonly DependencyProperty IsSeparatorProperty = DependencyProperty.Register(
            "IsSeparator", typeof (bool), typeof (ExtendedContextMenuItem), new PropertyMetadata(default(bool)));

        public bool IsSeparator
        {
            get => (bool) GetValue(IsSeparatorProperty);
            set => SetValue(IsSeparatorProperty, value);
        }

        #endregion

        #region Title

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof (string), typeof (ExtendedContextMenuItem), new PropertyMetadata(default(string)));

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion

        #region ShowCheckBox

        public static readonly DependencyProperty ShowCheckBoxProperty = DependencyProperty.Register(
            "ShowCheckBox", typeof (bool), typeof (ExtendedContextMenuItem), new PropertyMetadata(default(bool)));

        public bool ShowCheckBox
        {
            get => (bool) GetValue(ShowCheckBoxProperty);
            set => SetValue(ShowCheckBoxProperty, value);
        }

        #endregion

        #region CommandName

        public static readonly DependencyProperty CommandNameProperty = DependencyProperty.Register(
            "CommandName", typeof(string), typeof(ExtendedContextMenuItem), new PropertyMetadata(default(string)));

        public string CommandName
        {
            get => (string) GetValue(CommandNameProperty);
            set => SetValue(CommandNameProperty, value);
        }

        #endregion
        
        static ExtendedContextMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedContextMenuItem), new FrameworkPropertyMetadata(typeof(ExtendedContextMenuItem)));
        }
    }
}
