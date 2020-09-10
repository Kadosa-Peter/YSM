using System.Windows.Input;

namespace Ysm.Core
{
    public static class ModifierKeyHelper
    {
        public static bool IsCtrlDown => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

        public static bool IsShiftDown => Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);


        public static bool Any()
        {
            if (IsCtrlDown || IsShiftDown) return true;

            return false;
        }
    }
}
