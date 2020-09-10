using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ysm.Controls
{
    public class IconButton : Button
    {
        #region MouseLeaveIcon
        public static readonly DependencyProperty MouseLeaveIconProperty = DependencyProperty.Register
        ("MouseLeaveIcon", typeof(ImageSource), typeof(IconButton), new PropertyMetadata(default(ImageSource)));

        [Category("Icon")]
        public ImageSource MouseLeaveIcon
        {
            get => (ImageSource)GetValue(MouseLeaveIconProperty);
            set => SetValue(MouseLeaveIconProperty, value);
        } 
        #endregion

        #region MouseEnterIcon
        public static readonly DependencyProperty MouseEnterIconProperty = DependencyProperty.Register
          ("MouseEnterIcon", typeof(ImageSource), typeof(IconButton), new PropertyMetadata(default(ImageSource)));

        [Category("Icon")]
        public ImageSource MouseEnterIcon
        {
            get => (ImageSource)GetValue(MouseEnterIconProperty);
            set => SetValue(MouseEnterIconProperty, value);
        }
        #endregion

        #region ShowTooltip

        public static readonly DependencyProperty ShowTooltipProperty = DependencyProperty.Register(
            "ShowTooltip", typeof (bool), typeof (IconButton), new PropertyMetadata(default(bool)));

        public bool ShowTooltip
        {
            get => (bool) GetValue(ShowTooltipProperty);
            set => SetValue(ShowTooltipProperty, value);
        }

        #endregion

        static IconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconButton), new FrameworkPropertyMetadata(typeof(IconButton)));
        }

        protected override void OnToolTipOpening(ToolTipEventArgs e)
        {
            if (!ShowTooltip)
                e.Handled = true;

            base.OnToolTipOpening(e);
        }
    }
}
