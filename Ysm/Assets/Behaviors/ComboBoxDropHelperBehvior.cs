using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Ysm.Assets.Behaviors
{
    public class ComboBoxDropHelperBehvior : Behavior<ComboBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.DropDownOpened += (s, e) => { Kernel.Default.MenuIsOpen = true; };

            AssociatedObject.DropDownClosed += (s, e) => Task.Delay(500).GetAwaiter().OnCompleted(()=>Kernel.Default.MenuIsOpen = false);
        }
    }
}
