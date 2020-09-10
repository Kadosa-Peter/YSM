using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Ysm.Core;

namespace Ysm.Controls
{
    public class Folder : Control
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color", typeof(Color), typeof(Folder), new PropertyMetadata("#DCB679".ToColor()));

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set
            {
                SetValue(ColorProperty, value);
                System.Diagnostics.Debug.WriteLine(value);
            }
        }

        static Folder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Folder), new FrameworkPropertyMetadata(typeof(Folder)));
        }
    }
}
