using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Ysm.Core;

namespace Ysm.Controls
{
    [TemplatePart(Name = PART_Indicator, Type = typeof(Rectangle))]
    [TemplatePart(Name = PART_Track, Type = typeof(Canvas))]
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    public class ExtendedSlider : Control
    {
        private const string PART_Indicator = "PART_Indicator";
        private const string PART_Track = "PART_Track";
        private const string PART_TextBox = "PART_TextBox";

        private Rectangle indicator;
        private Canvas track;
        private TextBox textBox;

        #region DependencyProperties

        public static readonly DependencyProperty MinimumProperty;

        [Category("Common")]
        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty;
        [Category("Common")]
        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly DependencyProperty ValueProperty;

        [Category("Common")]
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty IntervalProperty;

        [Category("Common")]
        public double Interval
        {
            get => (double)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        public static readonly DependencyProperty IsIndicatotVisibleProperty;

        [Category("Common")]
        public bool IsIndicatotVisible
        {
            get => (bool)GetValue(IsIndicatotVisibleProperty);
            set => SetValue(IsIndicatotVisibleProperty, value);
        }

        public static readonly DependencyProperty AlginEdgeProperty;

        [Category("Common")]
        public bool AlginEdge
        {
            get => (bool)GetValue(AlginEdgeProperty);
            set => SetValue(AlginEdgeProperty, value);
        }

        public static readonly DependencyProperty ShowTooltipProperty;

        [Category("Layout")]
        public bool ShowTooltip
        {
            get => (bool)GetValue(ShowTooltipProperty);
            set => SetValue(ShowTooltipProperty, value);
        }

        public static readonly DependencyProperty IndicatorBrushProperty;

         [Category("Brush")]
        public SolidColorBrush IndicatorBrush
        {
            get => (SolidColorBrush) GetValue(IndicatorBrushProperty);
             set => SetValue(IndicatorBrushProperty, value);
         }

        #endregion

        /// <summary>
        /// Ha igaz, akkor aktiválom a TextBox-ot. 
        /// Igaz, ha nem kezdtem el állítani a Value értéket az Indicatorral.
        /// </summary>
        private bool _edit = true;

        /// <summary>
        /// A _startPoint a MouseLeftButtonDown eseményben kap értéket. 
        /// A _startPoint és a MouseMove eseménybe meghatározott p2 értékével számolom ki, hogy elindíthatom-e az Indicator mozgatását.
        /// </summary>
        private Point _startPoint;

        /// <summary>
        /// Minden egyes indicator mozgatás folyamat előtt el kell végezni bizonyos beállításokat, 
        /// de csak egyszer egy folyamat alatt. Az értéket a mlbu eseményben állítom igazra. 
        /// </summary>
        private bool indicatorMovementPreSettings = true;

        /// <summary>
        /// RepeatButton használatánál nem akarom kijelőlni a textbox-ban az értéket.
        /// </summary>
        private bool suppressTextSelect;

        private bool hasVisualTree;
        private bool isloaded;

        static ExtendedSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedSlider), new FrameworkPropertyMetadata(typeof(ExtendedSlider)));

            MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(ExtendedSlider), new PropertyMetadata(0.0));
            MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(ExtendedSlider), new PropertyMetadata(100.0, Maximum_PropertyChanged));
            IntervalProperty = DependencyProperty.Register("Interval", typeof(double), typeof(ExtendedSlider), new PropertyMetadata(1.0));
            ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ExtendedSlider), new PropertyMetadata(0.0, ValueProperty_ChangedCallback));
            IsIndicatotVisibleProperty = DependencyProperty.Register("IsIndicatotVisible", typeof(bool), typeof(ExtendedSlider), new PropertyMetadata(true));
            ShowTooltipProperty = DependencyProperty.Register("ShowTooltip", typeof(bool), typeof(ExtendedSlider), new PropertyMetadata(default(bool)));
            // Ha a max-nál nagyobb értéket adunk meg, akkor a maximumhoz igazodik, minimumnál hasonlóan.
            AlginEdgeProperty = DependencyProperty.Register("AlginEdge", typeof(bool), typeof(ExtendedSlider), new PropertyMetadata(default(bool)));
            IndicatorBrushProperty = DependencyProperty.Register("IndicatorBrush", typeof(SolidColorBrush), typeof(ExtendedSlider), new PropertyMetadata(Brushes.DimGray));
        }

        private static void Maximum_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            ExtendedSlider extendedSlider = d as ExtendedSlider;
            if (extendedSlider != null && extendedSlider.hasVisualTree)
            {
                double max = (double)e.NewValue;

                if (max < extendedSlider.Value)
                {
                    extendedSlider.Value = max;
                    extendedSlider.CalculateIndicatorWidth(max);
                }
                else
                {
                    extendedSlider.CalculateIndicatorWidth(extendedSlider.Value);
                }
            }
        }

        private static void ValueProperty_ChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double value = Convert.ToDouble(e.NewValue);
            ExtendedSlider slider = d as ExtendedSlider;

            if (slider != null && slider.isloaded)
            {
                if (value >= slider.Minimum && value <= slider.Maximum)
                {
                    slider.CalculateIndicatorWidth(value);
                }
            }
        }

        public ExtendedSlider()
        {
            Loaded += ExtendedSlider_Loaded;
        }

        private void ExtendedSlider_Loaded(object sender, RoutedEventArgs e)
        {
            if (hasVisualTree && !isloaded)
            {
                isloaded = true;

                if (Minimum > Value)
                    Value = Minimum;

                CalculateIndicatorWidth(Value);

                ToolTip = $"Min: {Minimum}\nMax: {Maximum}";
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            indicator = GetTemplateChild(PART_Indicator) as Rectangle;
            if(indicator != null)
            
            track = GetTemplateChild(PART_Track) as Canvas;
            
            textBox = GetTemplateChild(PART_TextBox) as TextBox;
            if(textBox != null)
            {
                textBox.KeyDown += TextBox_KeyDown;
                textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
                textBox.LostFocus += TextBox_LostFocus;
            }
            
            hasVisualTree = true;
        }

        protected override void OnToolTipOpening(ToolTipEventArgs e)
        {
            if (!ShowTooltip)
                e.Handled = true;

            base.OnToolTipOpening(e);
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    {
                        if (ModifierKeyHelper.IsShiftDown) // növelem 10-zel
                        {
                            double d = Value % 10.0;
                            d = 10.0 - d;
                            Increase(d);
                            textBox.SelectAll();
                            textBox.Focus();
                            e.Handled = true;

                        }
                        else // növelem 1-gyel
                        {
                            double d = Value % 1.0;
                            d = 1.0 - d;
                            Increase(d);
                            textBox.SelectAll();
                            textBox.Focus();
                            e.Handled = true;
                        }
                    }
                    break;
                case Key.Down:
                    {
                        if (ModifierKeyHelper.IsShiftDown) // csökkentem 10-zel
                        {
                            double d = Value % 10.0;
                            d = 10.0 - d;
                            Decrease(d);
                            textBox.SelectAll();
                            textBox.Focus();
                            e.Handled = true;
                        }
                        else // csökkentem 1-gyel
                        {
                            double d = Value % 1.0;
                            d = 1.0 - d;
                            Decrease(d);
                            textBox.SelectAll();
                            textBox.Focus();
                            e.Handled = true;
                        }
                    }
                    break;
            }

        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            CloseTextBox();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        CloseTextBox();
                    }
                    break;
                case Key.Enter:
                    {
                        CloseTextBox();
                    }
                    break;

            }
        }

        private void CloseTextBox()
        {
            double value;

            if (double.TryParse(textBox.Text, out value))
            {
                if (value >= Minimum && value <= Maximum)
                {
                    Value = value;
                    CalculateIndicatorWidth(Value);
                }
                else
                {

                    if (value > Maximum && AlginEdge)
                    {
                        Value = Maximum;
                        CalculateIndicatorWidth(Value);
                    }
                    else if (value < Minimum && AlginEdge)
                    {
                        Value = Minimum;
                        CalculateIndicatorWidth(Value);
                    }
                    else
                    {
                        textBox.Text = Value.ToString(CultureInfo.InvariantCulture);
                    }

                }
            }
            else
            {
                textBox.Text = Value.ToString(CultureInfo.InvariantCulture);
            }

            textBox.IsEnabled = false;

            indicator.Visibility = Visibility.Visible;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            _startPoint = e.GetPosition(this);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            if (_edit)
            {
                if (!suppressTextSelect)
                {
                    textBox.IsEnabled = true;
                    textBox.Visibility = Visibility.Visible;

                    indicator.Visibility = Visibility.Collapsed;
                    textBox.SelectAll();
                    textBox.Focus();
                }

                suppressTextSelect = false;

            }
            else
            {
                Cursor = Cursors.Arrow;
                CalculateCursorPosition();
                _edit = true;
                track.ReleaseMouseCapture();
                indicatorMovementPreSettings = true;
            }

        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            Point p2 = e.GetPosition(this);

            if (e.LeftButton == MouseButtonState.Pressed && DistanceLargerThanFive(p2) && textBox.IsEnabled == false)
            {
                if (indicatorMovementPreSettings)
                {
                    Cursor = Cursors.None;
                    CalculateCursorPosition();
                    indicatorMovementPreSettings = false;
                    track.CaptureMouse();
                }

                _edit = false;

                double width = e.GetPosition(this).X;

                if (width < 0) width = 0;
                if (width > track.ActualWidth) width = track.ActualWidth;

                indicator.Width = width;

                CalculateValue(width);
            }
        }

        private void CalculateValue(double width)
        {
            double percent = width / (track.ActualWidth / 100);
            double value = (Maximum - Minimum) / 100 * percent;
            value += Minimum;
            value = Math.Round(value, MidpointRounding.ToEven);
            textBox.Text = value.ToString(CultureInfo.InvariantCulture);
            Value = value;
        }

        private void CalculateIndicatorWidth(double value)
        {
            double one_percent = (Maximum - Minimum) / 100;
            double percent = (value - Minimum) / one_percent;
            double width = track.ActualWidth / 100 * percent;
            if (width < 0) width = 0;
            indicator.Width = width;
            textBox.Text = value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Igaz, ha kezdő és aktuális ponttól mért távolság nagyobb mint 5.
        /// </summary>
        /// <param name="p2">Aktuális egér pozíció a track felületén.</param>
        /// <returns></returns>
        private bool DistanceLargerThanFive(Point p2)
        {
            if (p2.X > _startPoint.X) // A kiindulási ponttól (_startPoint) jobbra mozdul el az egér.
            {
                if (p2.X - _startPoint.X > 5)
                    return true;

                return false;
            }

            if (_startPoint.X - p2.X > 5)
                return true;

            return false;
        }

        public void Increase(double value)
        {
            if (Value + value <= Maximum)
            {
                Value += value;
                CalculateIndicatorWidth(Value);
            }
            else
            {
                double d = Maximum - Value;

                if (d > 0)
                {
                    Value += d;
                    CalculateIndicatorWidth(Value);
                }
            }
        }

        public void Decrease(double value)
        {
            if (Value - value >= Minimum)
            {
                Value -= value;
                //if (Value%1 > 0) Value = Math.Round(0.1);
                CalculateIndicatorWidth(Value);
            }
            else
            {
                double d = Value - Minimum;

                if (d > 0)
                {
                    Value -= d;
                    // if (Value % 1 > 0) Value = Math.Round(0.1);
                    CalculateIndicatorWidth(Value);
                }
            }
        }

        private void CalculateCursorPosition()
        {
            if (IsIndicatotVisible)
            {
                Point p = PointToScreen(new Point());
                double top = p.Y + ActualHeight / 2;
                double left = p.X + indicator.ActualWidth;
                CursorPosition.Set((int)left, (int)top);
            }
            else
            {
                Point p = PointToScreen(new Point());
                double top = p.Y + _startPoint.Y;
                double left = p.X + _startPoint.X;
                CursorPosition.Set((int)left, (int)top);
            }

        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Tab && ModifierKeyHelper.IsCtrlDown)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Previous);
                MoveFocus(request);
                e.Handled = true;
            }
        }
    }

}
