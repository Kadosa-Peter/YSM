using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Ysm.Controls.HsvColor;
using Ysm.Core;

namespace Ysm.Controls
{
    [TemplatePart(Name = PART_SpectrumDisplay, Type = typeof(Rectangle))]
    [TemplatePart(Name = PART_Thumb, Type = typeof(Thumb))]
    public class ColorSpectrumSlider : Slider
    {
        private const string PART_SpectrumDisplay = "PART_SpectrumDisplay";
        private const string PART_Thumb = "PART_Thumb";
        private Thumb _thumb;

        #region Private Members

        private Rectangle _spectrumDisplay;
        private LinearGradientBrush _pickerBrush;

        #endregion //Private Members

        #region Constructors

        static ColorSpectrumSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSpectrumSlider), new FrameworkPropertyMetadata(typeof(ColorSpectrumSlider)));
        }

        #endregion //Constructors

        #region Dependency Properties

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorSpectrumSlider), new PropertyMetadata(Colors.Transparent));
        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        #endregion //Dependency Properties

        #region Base Class Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _spectrumDisplay = (Rectangle)GetTemplateChild(PART_SpectrumDisplay);
            _thumb = GetTemplateChild(PART_Thumb) as Thumb;
            CreateSpectrum();
            OnValueChanged(Double.NaN, Value);

        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);

            Color color = ColorUtilities.ConvertHsvToRgb(360 - newValue, 1, 1);
            SelectedColor = color;
        }

        #endregion //Base Class Overrides

        #region Methods

        private void CreateSpectrum()
        {
            _pickerBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0.5, 0),
                EndPoint = new Point(0.5, 1),
                ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation
            };

            List<Color> colorsList = ColorUtilities.GenerateHsvSpectrum();

            double stopIncrement = (double)1 / colorsList.Count;

            int i;
            for (i = 0; i < colorsList.Count; i++)
            {
                _pickerBrush.GradientStops.Add(new GradientStop(colorsList[i], i * stopIncrement));
            }

            _pickerBrush.GradientStops[i - 1].Offset = 1.0;
            _spectrumDisplay.Fill = _pickerBrush;
        }

        #endregion //Methods

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            _thumb.ForceFocus();
            _thumb.CaptureMouse();
            Mouse.Capture(_thumb);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            _thumb.ReleaseMouseCapture();
        }
    }
}
