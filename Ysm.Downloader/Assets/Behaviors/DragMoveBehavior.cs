using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Ysm.Downloader.Assets.Behaviors
{
    public class DragMoveBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Window window = Window.GetWindow(AssociatedObject);

                window?.DragMove();
            }
        }
    }
}
