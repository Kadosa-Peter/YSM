using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Ysm.Controls
{
    public class SideRadioButton : RadioButton
    {
        private Rectangle rectangle;
        private bool _isLoaded;

        #region Side

        public static readonly DependencyProperty SideProperty = DependencyProperty.Register(
            "Side", typeof (Side), typeof (SideRadioButton), new PropertyMetadata(default(Side)));

        [Category("Layout")]
        public Side Side
        {
            get => (Side) GetValue(SideProperty);
            set => SetValue(SideProperty, value);
        }

        #endregion

        static SideRadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SideRadioButton), new FrameworkPropertyMetadata(typeof(SideRadioButton)));
        }

        public SideRadioButton()
        {
            Loaded += SideRadioButton_Loaded;
        }

        private void SideRadioButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (Side == Side.Bottom || Side == Side.Top)
            {
                ScaleTransform scaleTransform = new ScaleTransform();
                scaleTransform.ScaleY = 1;

                if (IsChecked != null && IsChecked.Value)
                    scaleTransform.ScaleX = 1;
                else
                    scaleTransform.ScaleX = 0;

                TransformGroup group = new TransformGroup();
                group.Children.Add(scaleTransform);

                rectangle.RenderTransform = group;
            }
            else
            {
                ScaleTransform scaleTransform = new ScaleTransform();
                scaleTransform.ScaleX = 1;

                if (IsChecked != null && IsChecked.Value)
                    scaleTransform.ScaleY = 1;
                else
                    scaleTransform.ScaleY = 0;

                TransformGroup group = new TransformGroup();
                group.Children.Add(scaleTransform);

                rectangle.RenderTransform = group;

                // TODO: you should consider its border thickness
                //if (Side == Side.Right)
                //{
                //    rectangle.Margin = new Thickness(0,0,-1,0);
                //}
                //if (Side == Side.Left)
                //{
                //    rectangle.Margin = new Thickness(-1, 0, 0, 0);
                //}
            }

            _isLoaded = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            rectangle = GetTemplateChild("PART_Indicator") as Rectangle;
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            base.OnChecked(e);

            if (_isLoaded)
                if (Side == Side.Bottom || Side == Side.Top)
                {
                    Storyboard storyboard = (Storyboard)FindResource("WhenChecked_Horizontal");
                    storyboard.Begin(rectangle);
                }
                else
                {
                    Storyboard storyboard = (Storyboard)FindResource("WhenChecked_Vertical");
                    storyboard.Begin(rectangle);
                }
        }

        protected override void OnUnchecked(RoutedEventArgs e)
        {
            base.OnUnchecked(e);

            if (_isLoaded)
                if (Side == Side.Bottom || Side == Side.Top)
                {
                    Storyboard storyboard = (Storyboard)FindResource("WhenUnChecked_Horizontal");
                    storyboard.Begin(rectangle);

                }
                else
                {
                    Storyboard storyboard = (Storyboard)FindResource("WhenUnChecked_Vertical");
                    storyboard.Begin(rectangle);
                }
        }
    }


}
