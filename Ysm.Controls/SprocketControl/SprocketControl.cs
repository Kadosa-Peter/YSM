using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ysm.Controls
{
    public class SprocketControl : Control, IDisposable
    {
        #region Constants

        private const double DEFAULT_INTERVAL = 60;
        private static readonly Color DEFAULT_TICK_COLOR = Color.FromArgb(255, 58, 58, 58);
        private const double DEFAULT_TICK_WIDTH = 3;
        private const int DEFAULT_TICK_COUNT = 12;
        private const double MINIMUM_INNER_RADIUS = 5;
        private const double MINIMUM_OUTER_RADIUS = 8;
        private readonly Size MINIMUM_CONTROL_SIZE = new Size(28, 28);
        private const double DEFAULT_START_ANGLE = 270;
        private const double INNER_RADIUS_FACTOR = 0.175;
        private const double OUTER_RADIUS_FACTOR = 0.3125;
        // The Lower limit of the Alpha value (The spokes will be shown in 
        // alpha values ranging from 255 to m_AlphaLowerLimit)
        private const int ALPHA_UPPER_LIMIT = 250;
        private const int ALPHA_LOWER_LIMIT = 0;
        private const double ALPHA_TICK_PERCENTAGE_LOWER_LIMIT = 10;
        private const double DEFAULT_PROGRESS_ALPHA = 10;
        private const double DEFAULT_PROGRESS = 0.0;
        #endregion

        #region Enums

        /// <summary>
        /// Defines the Direction of Rotation
        /// </summary>
        public enum Direction
        {
            Clockwise,
            Anticlockwise
        }

        #endregion

        #region Structs

        /// <summary>
        /// Stores the details of each spoke
        /// </summary>
        struct Spoke
        {
            public readonly Point StartPoint;
            public readonly Point EndPoint;

            public Spoke(Point pt1, Point pt2)
            {
                StartPoint = pt1;
                EndPoint = pt2;
            }
        }

        #endregion

        #region Fields

        Point _centerPoint;
        double _innerRadius;
        double _outerRadius;
        double _alphaChange;
        double _angleIncrement;
        double _renderStartAngle;
        readonly Timer _renderTimer;
        List<Spoke> _spokes;

        #endregion

        #region Dependency Properties

        #region Interval

        /// <summary>
        /// Interval Dependency Property
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(double), typeof(SprocketControl),
                new FrameworkPropertyMetadata(DEFAULT_INTERVAL,
                                              OnIntervalChanged));

        /// <summary>
        /// Gets or sets the Interval property. This dependency property 
        /// indicates duration at which the timer for rotation should fire.
        /// </summary>
        [Category("Sprocket")]
        public double Interval
        {
            get => (double)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        /// <summary>
        /// Handles changes to the Interval property.
        /// </summary>
        /// <param name="d">SprocketControl</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SprocketControl sprocket = (SprocketControl)d;
            double oldInterval = (double)e.OldValue;
            double newInterval = sprocket.Interval;
            sprocket.OnIntervalChanged(oldInterval, newInterval);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Interval property.
        /// </summary>
        /// <param name="oldInterval">Old Value</param>
        /// <param name="newInterval">New Value</param>
        protected virtual void OnIntervalChanged(double oldInterval, double newInterval)
        {
            if (_renderTimer != null)
            {
                bool isEnabled = _renderTimer.Enabled;
                _renderTimer.Enabled = false;
                _renderTimer.Interval = newInterval;
                _renderTimer.Enabled = isEnabled;
            }
        }

        #endregion

        #region IsIndeterminate

        /// <summary>
        /// IsIndeterminate Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(SprocketControl),
                new FrameworkPropertyMetadata(true,
                                              OnIsIndeterminateChanged));

        /// <summary>
        /// Gets or sets the IsIndeterminate property. This dependency property 
        /// indicates whether the SprocketControl's progress is indeterminate or not.
        /// </summary>
        [Category("Sprocket")]
        public bool IsIndeterminate
        {
            get => (bool)GetValue(IsIndeterminateProperty);
            set => SetValue(IsIndeterminateProperty, value);
        }

        /// <summary>
        /// Handles changes to the IsIndeterminate property.
        /// </summary>
        /// <param name="d">SprocketControl</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnIsIndeterminateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SprocketControl target = (SprocketControl)d;
            bool oldIsIndeterminate = (bool)e.OldValue;
            bool newIsIndeterminate = target.IsIndeterminate;
            target.OnIsIndeterminateChanged(oldIsIndeterminate, newIsIndeterminate);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the IsIndeterminate property.
        /// </summary>
        /// <param name="oldIsIndeterminate">Old Value</param>
        /// <param name="newIsIndeterminate">New Value</param>
        protected virtual void OnIsIndeterminateChanged(bool oldIsIndeterminate, bool newIsIndeterminate)
        {
            if (oldIsIndeterminate != newIsIndeterminate)
            {
                if (newIsIndeterminate && IsVisible)
                {
                    // Start the renderTimer
                    Start();
                }
                else
                {
                    // Stop the renderTimer
                    Stop();
                    InvalidateVisual();
                }
            }
        }

        #endregion

        #region Progress

        /// <summary>
        /// Progress Dependency Property
        /// </summary>
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(SprocketControl),
                new FrameworkPropertyMetadata(DEFAULT_PROGRESS,
                                              OnProgressChanged,
                                              CoerceProgress));

        /// <summary>
        /// Gets or sets the Progress property. This dependency property 
        /// indicates the progress percentage.
        /// </summary>
        [Category("Sprocket")]
        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        /// <summary>
        /// Coerces the Progress value so that it stays in the range 0-100
        /// </summary>
        /// <param name="d">SprocketControl</param>
        /// <param name="value">New Value</param>
        /// <returns>Coerced Value</returns>
        private static object CoerceProgress(DependencyObject d, object value)
        {
            double progress = (double)value;

            if (progress < 0.0)
            {
                return 0.0;
            }

            if (progress > 100.0)
            {
                return 100.0;
            }

            return value;
        }

        /// <summary>
        /// Handles changes to the Progress property.
        /// </summary>
        /// <param name="d">SprocketControl</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SprocketControl sprocket = (SprocketControl)d;
            double oldProgress = (double)e.OldValue;
            double newProgress = sprocket.Progress;
            sprocket.OnProgressChanged(oldProgress, newProgress);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Progress property.
        /// </summary>
        /// <param name="oldProgress">Old Value</param>
        /// <param name="newProgress">New Value</param>
        protected virtual void OnProgressChanged(double oldProgress, double newProgress)
        {
            InvalidateVisual();
        }

        #endregion

        #region Rotation

        /// <summary>
        /// Rotation Dependency Property
        /// </summary>
        public static readonly DependencyProperty RotationProperty =
            DependencyProperty.Register("Rotation", typeof(Direction), typeof(SprocketControl),
                new FrameworkPropertyMetadata(Direction.Clockwise,
                                              OnRotationChanged));

        /// <summary>
        /// Gets or sets the Rotation property. This dependency property 
        /// indicates the direction of Rotation of the SprocketControl.
        /// </summary>
        [Category("Sprocket")]
        public Direction Rotation
        {
            get => (Direction)GetValue(RotationProperty);
            set => SetValue(RotationProperty, value);
        }

        /// <summary>
        /// Handles changes to the Rotation property.
        /// </summary>
        /// <param name="d">SprocketControl</param>
        /// <param name="e">DependencyProperty changed event arguments</param>

        private static void OnRotationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SprocketControl sprocket = (SprocketControl)d;
            Direction oldRotation = (Direction)e.OldValue;
            Direction newRotation = sprocket.Rotation;
            sprocket.OnRotationChanged(oldRotation, newRotation);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Rotation property.
        /// </summary>
        /// <param name="oldRotation">Old Value</param>
        /// <param name="newRotation">New Value</param>
        protected virtual void OnRotationChanged(Direction oldRotation, Direction newRotation)
        {
            // Recalculate the spoke points
            CalculateSpokesPoints();
        }

        #endregion

        #region StartAngle

        /// <summary>
        /// StartAngle Dependency Property
        /// </summary>
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(SprocketControl),
                new FrameworkPropertyMetadata(DEFAULT_START_ANGLE,
                                              OnStartAngleChanged));

        /// <summary>
        /// Gets or sets the StartAngle property. This dependency property 
        /// indicates the angle at which the first spoke (with max opacity) is drawn.
        /// </summary>
        [Category("Sprocket")]
        public double StartAngle
        {
            get => (double)GetValue(StartAngleProperty);
            set => SetValue(StartAngleProperty, value);
        }

        /// <summary>
        /// Handles changes to the StartAngle property.
        /// </summary>
        /// <param name="d">SprocketControl</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnStartAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SprocketControl sprocket = (SprocketControl)d;
            double oldStartAngle = (double)e.OldValue;
            double newStartAngle = sprocket.StartAngle;
            sprocket.OnStartAngleChanged(oldStartAngle, newStartAngle);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the StartAngle property.
        /// </summary>
        /// <param name="oldStartAngle">Old Value</param>
        /// <param name="newStartAngle">New Value</param>
        protected virtual void OnStartAngleChanged(double oldStartAngle, double newStartAngle)
        {
            // Recalculate the spoke points
            CalculateSpokesPoints();
        }

        #endregion

        #region TickColor

        /// <summary>
        /// TickColor Dependency Property
        /// </summary>
        public static readonly DependencyProperty TickColorProperty =
            DependencyProperty.Register("TickColor", typeof(Color), typeof(SprocketControl),
                new FrameworkPropertyMetadata(DEFAULT_TICK_COLOR,
                                              OnTickColorChanged));

        /// <summary>
        /// Gets or sets the TickColor property. This dependency property 
        /// indicates the color of the Spokes in the SprocketControl.
        /// </summary>
        [Category("Sprocket")]
        public Color TickColor
        {
            get => (Color)GetValue(TickColorProperty);
            set => SetValue(TickColorProperty, value);
        }

        /// <summary>
        /// Handles changes to the TickColor property.
        /// </summary>
        /// <param name="d">SprocketControl</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnTickColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SprocketControl sprocket = (SprocketControl)d;
            Color oldTickColor = (Color)e.OldValue;
            Color newTickColor = sprocket.TickColor;
            sprocket.OnTickColorChanged(oldTickColor, newTickColor);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the TickColor property.
        /// </summary>
        /// <param name="oldTickColor">Old Value</param>
        /// <param name="newTickColor">New Value</param>
        protected virtual void OnTickColorChanged(Color oldTickColor, Color newTickColor)
        {
            InvalidateVisual();
        }

        #endregion

        #region TickCount

        /// <summary>
        /// TickCount Dependency Property
        /// </summary>
        public static readonly DependencyProperty TickCountProperty =
            DependencyProperty.Register("TickCount", typeof(int), typeof(SprocketControl),
                new FrameworkPropertyMetadata(DEFAULT_TICK_COUNT,
                                              OnTickCountChanged,
                                              CoerceTickCount));

        /// <summary>
        /// Gets or sets the TickCount property. This dependency property 
        /// indicates the number of spokes of the SprocketControl.
        /// </summary>
        [Category("Sprocket")]
        public int TickCount
        {
            get => (int)GetValue(TickCountProperty);
            set => SetValue(TickCountProperty, value);
        }

        /// <summary>
        /// Coerces the TickCount value to an acceptable value
        /// </summary>
        /// <param name="d">SprocketControl</param>
        /// <param name="value">New Value</param>
        /// <returns>Coerced Value</returns>
        private static object CoerceTickCount(DependencyObject d, object value)
        {
            int count = (int)value;

            if (count <= 0)
            {
                return DEFAULT_TICK_COUNT;
            }

            return value;
        }

        /// <summary>
        /// Handles changes to the TickCount property.
        /// </summary>
        /// <param name="d">SprocketControl</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnTickCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SprocketControl sprocket = (SprocketControl)d;
            int oldTickCount = (int)e.OldValue;
            int newTickCount = sprocket.TickCount;
            sprocket.OnTickCountChanged(oldTickCount, newTickCount);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the TickCount property.
        /// </summary>
        /// <param name="oldTickCount">Old Value</param>
        /// <param name="newTickCount">New Value</param>
        protected virtual void OnTickCountChanged(int oldTickCount, int newTickCount)
        {
            // Recalculate the spoke points
            CalculateSpokesPoints();
        }

        #endregion

        #region TickStyle

        /// <summary>
        /// TickStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty TickStyleProperty =
            DependencyProperty.Register("TickStyle", typeof(PenLineCap), typeof(SprocketControl),
                new FrameworkPropertyMetadata(PenLineCap.Round, OnTickStyleChanged));

        /// <summary>
        /// Gets or sets the TickStyle property. This dependency property 
        /// indicates the style of the ends of each tick.
        /// </summary>
        [Category("Sprocket")]
        public PenLineCap TickStyle
        {
            get => (PenLineCap)GetValue(TickStyleProperty);
            set => SetValue(TickStyleProperty, value);
        }

        /// <summary>
        /// Handles changes to the TickStyle property.
        /// </summary>
        /// <param name="d">SprocketControl</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnTickStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SprocketControl sprocket = (SprocketControl)d;
            PenLineCap oldTickStyle = (PenLineCap)e.OldValue;
            PenLineCap newTickStyle = sprocket.TickStyle;
            sprocket.OnTickStyleChanged(oldTickStyle, newTickStyle);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the TickStyle property.
        /// </summary>
        /// <param name="oldTickStyle">Old Value</param>
        /// <param name="newTickStyle">New Value</param>
        protected virtual void OnTickStyleChanged(PenLineCap oldTickStyle, PenLineCap newTickStyle)
        {
            InvalidateVisual();
        }

        #endregion

        #region TickWidth

        /// <summary>
        /// TickWidth Dependency Property
        /// </summary>
        public static readonly DependencyProperty TickWidthProperty =
            DependencyProperty.Register("TickWidth", typeof(double), typeof(SprocketControl),
                new FrameworkPropertyMetadata(DEFAULT_TICK_WIDTH,
                                              OnTickWidthChanged,
                                              CoerceTickWidth));

        /// <summary>
        /// Gets or sets the TickWidth property. This dependency property 
        /// indicates the width of each spoke in the SprocketControl.
        /// </summary>
        [Category("Sprocket")]
        public double TickWidth
        {
            get => (double)GetValue(TickWidthProperty);
            set => SetValue(TickWidthProperty, value);
        }

        /// <summary>
        /// Coerces the TickWidth value so that it stays above 0.
        /// </summary>
        /// <param name="d">SprocketControl</param>
        /// <param name="value">New Value</param>
        /// <returns>Coerced Value</returns>
        private static object CoerceTickWidth(DependencyObject d, object value)
        {
            double progress = (double)value;

            if (progress < 0.0)
            {
                return DEFAULT_TICK_WIDTH;
            }

            return value;
        }
        /// <summary>
        /// Handles changes to the TickWidth property.
        /// </summary>
        /// <param name="d">SprocketControl</param>
        /// <param name="e">DependencyProperty changed event arguments</param>
        private static void OnTickWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SprocketControl target = (SprocketControl)d;
            double oldTickWidth = (double)e.OldValue;
            double newTickWidth = target.TickWidth;
            target.OnTickWidthChanged(oldTickWidth, newTickWidth);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the TickWidth property.
        /// </summary>
        /// <param name="oldTickWidth">Old Value</param>
        /// <param name="newTickWidth">New Value</param>
        protected virtual void OnTickWidthChanged(double oldTickWidth, double newTickWidth)
        {
            InvalidateVisual();
        }

        #endregion

        #region LowestAlpha

        /// <summary>
        /// LowestAlpha Dependency Property
        /// </summary>
        public static readonly DependencyProperty LowestAlphaProperty =
            DependencyProperty.Register("LowestAlpha", typeof(Int32), typeof(SprocketControl),
                new FrameworkPropertyMetadata(ALPHA_LOWER_LIMIT,
                    OnLowestAlphaChanged,
                    CoerceLowestAlpha));

        /// <summary>
        /// Gets or sets the LowestAlpha property. This dependency property 
        /// indicates the lowest Opacity value that must be used while rendering the SprocketControl's spokes.
        /// </summary>
        [Category("Sprocket")]
        public Int32 LowestAlpha
        {
            get => (Int32)GetValue(LowestAlphaProperty);
            set => SetValue(LowestAlphaProperty, value);
        }

        /// <summary>
        /// Handles changes to the LowestAlpha property.
        /// </summary>
        private static void OnLowestAlphaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SprocketControl sprocket = (SprocketControl)d;
            Int32 oldLowestAlpha = (Int32)e.OldValue;
            Int32 newLowestAlpha = sprocket.LowestAlpha;
            sprocket.OnLowestAlphaChanged(oldLowestAlpha, newLowestAlpha);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the LowestAlpha property.
        /// </summary>
        protected virtual void OnLowestAlphaChanged(Int32 oldLowestAlpha, Int32 newLowestAlpha)
        {
        }

        /// <summary>
        /// Coerces the LowestAlpha value.
        /// </summary>
        private static object CoerceLowestAlpha(DependencyObject d, object value)
        {
            Int32 desiredLowestAlpha = (Int32)value;

            if (desiredLowestAlpha < ALPHA_LOWER_LIMIT)
                return ALPHA_LOWER_LIMIT;
            else if (desiredLowestAlpha > ALPHA_UPPER_LIMIT)
                return ALPHA_UPPER_LIMIT;

            return desiredLowestAlpha;
        }

        #endregion

        #region AlphaTicksPercentage

        /// <summary>
        /// AlphaTicksPercentage Dependency Property
        /// </summary>
        public static readonly DependencyProperty AlphaTicksPercentageProperty =
            DependencyProperty.Register("AlphaTicksPercentage", typeof(double), typeof(SprocketControl),
                new FrameworkPropertyMetadata(100.0,
                    OnAlphaTicksPercentageChanged,
                    CoerceAlphaTicksPercentage));

        /// <summary>
        /// Gets or sets the AlphaTicksPercentage property. This dependency property 
        /// indicates the percentage of total ticks which must be considered for step by step reduction
        /// of the alpha value. The remaining ticks remain at the LowestAlpha value.
        /// </summary>
       [Category("Sprocket")]
        public double AlphaTicksPercentage
        {
            get => (double)GetValue(AlphaTicksPercentageProperty);
            set => SetValue(AlphaTicksPercentageProperty, value);
        }

        /// <summary>
        /// Handles changes to the AlphaTicksPercentage property.
        /// </summary>
        private static void OnAlphaTicksPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SprocketControl sprocket = (SprocketControl)d;
            double oldAlphaTicksPercentage = (double)e.OldValue;
            double newAlphaTicksPercentage = sprocket.AlphaTicksPercentage;
            sprocket.OnAlphaTicksPercentageChanged(oldAlphaTicksPercentage, newAlphaTicksPercentage);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the AlphaTicksPercentage property.
        /// </summary>
        protected virtual void OnAlphaTicksPercentageChanged(double oldAlphaTicksPercentage, double newAlphaTicksPercentage)
        {
        }

        /// <summary>
        /// Coerces the AlphaTicksPercentage value.
        /// </summary>
        private static object CoerceAlphaTicksPercentage(DependencyObject d, object value)
        {
            double desiredAlphaTicksPercentage = (double)value;

            if (desiredAlphaTicksPercentage > 100.0)
                return 100.0;
            if (desiredAlphaTicksPercentage < ALPHA_TICK_PERCENTAGE_LOWER_LIMIT)
                return ALPHA_TICK_PERCENTAGE_LOWER_LIMIT;

            return desiredAlphaTicksPercentage;
        }

        #endregion

        #endregion

        #region Construction

        /// <summary>
        /// Ctor
        /// </summary>
        public SprocketControl()
        {
            _renderTimer = new Timer(Interval);
            _renderTimer.Elapsed += OnRenderTimerElapsed;

            // Set the minimum size of the SprocketControl
            MinWidth = MINIMUM_CONTROL_SIZE.Width;
            MinWidth = MINIMUM_CONTROL_SIZE.Height;

            // Calculate the spoke points based on the current size
            CalculateSpokesPoints();

            void Run(object sender, RoutedEventArgs e)
            {
                Loaded -= Run;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (IsIndeterminate && IsVisible) Start();
                }));
            }

            Loaded += Run;

            // Event handler added to stop the timer if the control is no longer visible
            IsVisibleChanged += OnVisibilityChanged;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Start the Tick Control rotation
        /// </summary>
        private void Start()
        {
            if ((_renderTimer != null) && !_renderTimer.Enabled)
            {
                _renderTimer.Interval = Interval;
                _renderTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Stop the Tick Control rotation
        /// </summary>
        private void Stop()
        {
            if (_renderTimer != null)
            {
                _renderTimer.Enabled = false;
            }
        }

        /// <summary>
        /// Converts Degrees to Radians
        /// </summary>
        /// <param name="degrees">Degrees</param>
        /// <returns>Radians</returns>
        private double ConvertDegreesToRadians(double degrees)
        {
            return Math.PI / 180 * degrees;
        }

        /// <summary>
        /// Calculate the Spoke Points and store them
        /// </summary>
        private void CalculateSpokesPoints()
        {
            _spokes = new List<Spoke>();

            // Calculate the angle between adjacent spokes
            _angleIncrement = 360 / (double)TickCount;
            // Calculate the change in alpha between adjacent spokes
            _alphaChange = (int)((255 - LowestAlpha) / (AlphaTicksPercentage / 100.0 * TickCount));

            // Set the start angle for rendering
            _renderStartAngle = StartAngle;

            // Calculate the location around which the spokes will be drawn
            double width = Width < Height ? Width : Height;
            _centerPoint = new Point(Width / 2, Height / 2);
            // Calculate the inner and outer radii of the control. The radii should not be less than the
            // Minimum values
            _innerRadius = (int)(width * INNER_RADIUS_FACTOR);
            if (_innerRadius < MINIMUM_INNER_RADIUS)
                _innerRadius = MINIMUM_INNER_RADIUS;
            _outerRadius = (int)(width * OUTER_RADIUS_FACTOR);
            if (_outerRadius < MINIMUM_OUTER_RADIUS)
                _outerRadius = MINIMUM_OUTER_RADIUS;

            double angle = 0;

            for (int i = 0; i < TickCount; i++)
            {
                Point pt1 = new Point(_innerRadius * (float)Math.Cos(ConvertDegreesToRadians(angle)), _innerRadius * (float)Math.Sin(ConvertDegreesToRadians(angle)));
                Point pt2 = new Point(_outerRadius * (float)Math.Cos(ConvertDegreesToRadians(angle)), _outerRadius * (float)Math.Sin(ConvertDegreesToRadians(angle)));

                // Create a spoke based on the points generated
                Spoke spoke = new Spoke(pt1, pt2);
                // Set the spoke to the List
                _spokes.Add(spoke);

                // If it is not it Indeterminate state, 
                // ensure that the spokes are drawn in clockwise manner
                if (!IsIndeterminate)
                {
                    angle += _angleIncrement;
                }
                else
                {
                    if (Rotation == Direction.Clockwise)
                    {
                        angle -= _angleIncrement;
                    }
                    else if (Rotation == Direction.Anticlockwise)
                    {
                        angle += _angleIncrement;
                    }
                }
            }
        }

        #endregion

        #region Overrides

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            // Calculate the spoke points based on the new size
            CalculateSpokesPoints();
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (_spokes == null)
                return;

            TranslateTransform translate = new TranslateTransform(_centerPoint.X, _centerPoint.Y);
            dc.PushTransform(translate);
            RotateTransform rotate = new RotateTransform(_renderStartAngle);
            dc.PushTransform(rotate);

            byte alpha = 255;

            // Get the number of spokes that can be drawn with zero transparency
            int progressSpokes = (int)Math.Floor(Progress * TickCount / 100.0);

            // Render the spokes
            for (int i = 0; i < TickCount; i++)
            {
                if (!IsIndeterminate)
                {
                    if (progressSpokes > 0)
                        alpha = (byte)(i < progressSpokes ? 255 : DEFAULT_PROGRESS_ALPHA);
                    else
                        alpha = (byte)DEFAULT_PROGRESS_ALPHA;
                }

                Pen p = new Pen(new SolidColorBrush(Color.FromArgb(alpha, TickColor.R, TickColor.G, TickColor.B)), TickWidth);
                p.StartLineCap = p.EndLineCap = TickStyle;
                dc.DrawLine(p, _spokes[i].StartPoint, _spokes[i].EndPoint);

                if (IsIndeterminate)
                {
                    alpha -= (byte)_alphaChange;
                    if (alpha < LowestAlpha)
                        alpha = (byte)LowestAlpha;
                }
            }

            // Perform a reverse Rotation and Translation to obtain the original Transformation
            dc.Pop();
            dc.Pop();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the Elapsed event of the renderTimer
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        void OnRenderTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (Rotation == Direction.Clockwise)
                {
                    _renderStartAngle += _angleIncrement;

                    if (_renderStartAngle >= 360)
                        _renderStartAngle -= 360;
                }
                else if (Rotation == Direction.Anticlockwise)
                {
                    _renderStartAngle -= _angleIncrement;

                    if (_renderStartAngle <= -360)
                        _renderStartAngle += 360;
                }

                // Force re-rendering of control
                InvalidateVisual();
            }));
        }

        /// <summary>
        /// Event handler to stop the timer if the control is no longer visible
        /// and start it when it is visible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Needs to be handled only if the state of the progress bar is indeterminate
            if (IsIndeterminate)
            {
                if ((bool)e.NewValue)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Releases all resources used by an instance of the SprocketControl class.
        /// </summary>
        /// <remarks>
        /// This method calls the virtual Dispose(bool) method, passing in 'true', and then suppresses 
        /// finalization of the instance.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged resources before an instance of the SprocketControl class is reclaimed by garbage collection.
        /// </summary>
        /// <remarks>
        /// NOTE: Leave out the finalizer altogether if this class doesn't own unmanaged resources itself, 
        /// but leave the other methods exactly as they are.
        /// This method releases unmanaged resources by calling the virtual Dispose(bool), passing in 'false'.
        /// </remarks>
        ~SprocketControl()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases the unmanaged resources used by an instance of the SprocketControl class and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">'true' to release both managed and unmanaged resources; 'false' to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_renderTimer != null)
                {
                    _renderTimer.Elapsed -= OnRenderTimerElapsed;
                    _renderTimer.Dispose();
                }
                IsVisibleChanged -= OnVisibilityChanged;
            }

            // free native resources if there are any.			
        }

        #endregion
    }
}
