using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Ysm.Core;

namespace Ysm.Assets.Behaviors
{
    public class DragMoveBehaviour : Behavior<FrameworkElement>
    {
        private Window _window;

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject != null)
            {
                AssociatedObject.MouseLeftButtonDown += Element_MouseLeftButtonDown;
                AssociatedObject.MouseLeftButtonUp += Element_MouseLeftButtonUp;
                AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            }
        }

        private void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _window = (sender as DependencyObject).GetParentOfType<Window>();
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                _window?.DragMove();
        }

        private void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _window = null;
        }
    }
}
