using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Ysm.Controls
{
    public class IconRepeatButton : RepeatButton
    {
        #region MouseLeaveIcon
        public static readonly DependencyProperty MouseLeaveIconProperty = DependencyProperty.Register
            ("MouseLeaveIcon", typeof(ImageSource), typeof(IconRepeatButton), new PropertyMetadata(default(ImageSource)));

        [Category("Icon")]
        public ImageSource MouseLeaveIcon
        {
            get => (ImageSource)GetValue(MouseLeaveIconProperty);
            set => SetValue(MouseLeaveIconProperty, value);
        }
        #endregion

        #region MouseEnterIcon
        public static readonly DependencyProperty MouseEnterIconProperty = DependencyProperty.Register
            ("MouseEnterIcon", typeof(ImageSource), typeof(IconRepeatButton), new PropertyMetadata(default(ImageSource)));

        [Category("Icon")]
        public ImageSource MouseEnterIcon
        {
            get => (ImageSource)GetValue(MouseEnterIconProperty);
            set => SetValue(MouseEnterIconProperty, value);
        }
        #endregion

        #region ShowTooltip

        public static readonly DependencyProperty ShowTooltipProperty = DependencyProperty.Register(
            "ShowTooltip", typeof(bool), typeof(IconRepeatButton), new PropertyMetadata(default(bool)));

        public bool ShowTooltip
        {
            get => (bool)GetValue(ShowTooltipProperty);
            set => SetValue(ShowTooltipProperty, value);
        }

        #endregion

        static IconRepeatButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconRepeatButton), new FrameworkPropertyMetadata(typeof(IconRepeatButton)));
        }

        protected override void OnToolTipOpening(ToolTipEventArgs e)
        {
            if (!ShowTooltip)
                e.Handled = true;

            base.OnToolTipOpening(e);
        }
    }
}
