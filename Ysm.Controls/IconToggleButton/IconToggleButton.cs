using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Ysm.Controls
{
    public class IconToggleButton : ToggleButton
    {
        #region OnEnter
        public static readonly DependencyProperty OnEnterProperty = DependencyProperty.Register
          ("OnEnter", typeof(ImageSource), typeof(IconToggleButton), new PropertyMetadata(default(ImageSource)));

        [Category("Icon")]
        public ImageSource OnEnter
        {
            get => (ImageSource)GetValue(OnEnterProperty);
            set => SetValue(OnEnterProperty, value);
        } 
        #endregion

        #region OnLeave
        public static readonly DependencyProperty OnLeaveProperty = DependencyProperty.Register
         ("OnLeave", typeof(ImageSource), typeof(IconToggleButton), new PropertyMetadata(default(ImageSource)));

        [Category("Icon")]
        public ImageSource OnLeave
        {
            get => (ImageSource)GetValue(OnLeaveProperty);
            set => SetValue(OnLeaveProperty, value);
        } 
        #endregion

        #region OffEnter
        public static readonly DependencyProperty OffEnterProperty = DependencyProperty.Register
           ("OffEnter", typeof(ImageSource), typeof(IconToggleButton), new PropertyMetadata(default(ImageSource)));

        [Category("Icon")]
        public ImageSource OffEnter
        {
            get => (ImageSource)GetValue(OffEnterProperty);
            set => SetValue(OffEnterProperty, value);
        } 
        #endregion

        #region OffLeave
        public static readonly DependencyProperty OffLeaveProperty = DependencyProperty.Register("OffLeave", typeof(ImageSource), typeof(IconToggleButton), new PropertyMetadata(default(ImageSource)));

        [Category("Icon")]
        public ImageSource OffLeave
        {
            get => (ImageSource)GetValue(OffLeaveProperty);
            set => SetValue(OffLeaveProperty, value);
        } 
        #endregion

        static IconToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconToggleButton), new FrameworkPropertyMetadata(typeof(IconToggleButton)));
        }
    }
}
