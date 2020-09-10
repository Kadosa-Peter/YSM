using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ysm.Controls
{
    public class DropDown : ComboBox
    {
        #region EnterIcon

        public static readonly DependencyProperty EnterIconProperty = DependencyProperty.Register(
            "EnterIcon", typeof(ImageSource), typeof(DropDown), new PropertyMetadata(default(ImageSource)));

        public ImageSource EnterIcon
        {
            get => (ImageSource) GetValue(EnterIconProperty);
            set => SetValue(EnterIconProperty, value);
        }

        #endregion

        #region LeaveIcon

        public static readonly DependencyProperty LeaveIconProperty = DependencyProperty.Register(
            "LeaveIcon", typeof(ImageSource), typeof(DropDown), new PropertyMetadata(default(ImageSource)));

        public ImageSource LeaveIcon
        {
            get => (ImageSource) GetValue(LeaveIconProperty);
            set => SetValue(LeaveIconProperty, value);
        }

        #endregion

        static DropDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDown), new FrameworkPropertyMetadata(typeof(DropDown)));
        }
    }
}
