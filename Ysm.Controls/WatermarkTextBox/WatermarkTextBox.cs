using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ysm.Controls
{
    public class WatermarkTextBox : TextBox
    {
        #region Watermark

        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register
            ("Watermark", typeof(string), typeof(WatermarkTextBox), new PropertyMetadata("Watermark"));

        public string Watermark
        {
            get => (string)GetValue(WatermarkProperty);
            set => SetValue(WatermarkProperty, value);
        }

        #endregion

        #region WatermarkIsEnabled

        public static readonly DependencyProperty WatermarkIsEnabledProperty = DependencyProperty.Register(
            "WatermarkIsEnabled", typeof (bool), typeof (WatermarkTextBox), new PropertyMetadata(true));

        public bool WatermarkIsEnabled
        {
            get => (bool) GetValue(WatermarkIsEnabledProperty);
            set => SetValue(WatermarkIsEnabledProperty, value);
        }

        #endregion

        #region ShowWatermark

        public static readonly DependencyProperty ShowWatermarkProperty = DependencyProperty.Register
            ("ShowWatermark", typeof(bool), typeof(WatermarkTextBox), new PropertyMetadata(true));

        public bool ShowWatermark
        {
            get => (bool)GetValue(ShowWatermarkProperty);
            set => SetValue(ShowWatermarkProperty, value);
        }

        #endregion

        #region WatermarkForeground

        public static readonly DependencyProperty WatermarkForegroundProperty = DependencyProperty.Register
            ("WatermarkForeground", typeof(SolidColorBrush), typeof(WatermarkTextBox), new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush WatermarkForeground
        {
            get => (SolidColorBrush)GetValue(WatermarkForegroundProperty);
            set => SetValue(WatermarkForegroundProperty, value);
        }

        #endregion

        #region WatermarkVerticalAlignment

        public static readonly DependencyProperty WatermarkVerticalAlignmentProperty = DependencyProperty.Register
            ("WatermarkVerticalAlignment", typeof(VerticalAlignment), typeof(WatermarkTextBox), new PropertyMetadata(default(VerticalAlignment)));

        public VerticalAlignment WatermarkVerticalAlignment
        {
            get => (VerticalAlignment)GetValue(WatermarkVerticalAlignmentProperty);
            set => SetValue(WatermarkVerticalAlignmentProperty, value);
        }

        #endregion

        #region WatermarkHorizontalAlignment

        public static readonly DependencyProperty WatermarkHorizontalAlignmentProperty = DependencyProperty.Register
            ("WatermarkHorizontalAlignment", typeof(HorizontalAlignment), typeof(WatermarkTextBox), new PropertyMetadata(default(HorizontalAlignment)));

        public HorizontalAlignment WatermarkHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(WatermarkHorizontalAlignmentProperty);
            set => SetValue(WatermarkHorizontalAlignmentProperty, value);
        }

        #endregion

        #region WatermarkPadding

        public static readonly DependencyProperty WatermarkPaddingProperty = DependencyProperty.Register
            ("WatermarkPadding", typeof(Thickness), typeof(WatermarkTextBox), new PropertyMetadata(default(Thickness)));

        public Thickness WatermarkPadding
        {
            get => (Thickness)GetValue(WatermarkPaddingProperty);
            set => SetValue(WatermarkPaddingProperty, value);
        }

        #endregion

        static WatermarkTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WatermarkTextBox), new FrameworkPropertyMetadata(typeof(WatermarkTextBox)));
        }

        public WatermarkTextBox()
        {
            Loaded += (k, l) => SetWatermarkVisibility();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            SetWatermarkVisibility();
        }

        private void SetWatermarkVisibility()
        {
            if (string.IsNullOrEmpty(Text))
            {
                ShowWatermark = true;
            }
            else
            {
                ShowWatermark = false;
            }
        }
    }
}
