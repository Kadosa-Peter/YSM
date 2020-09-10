using System;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Ysm.Core
{
    public static class InputHelper
    {
        public static bool IsInputFocused()
        {
            try
            {
                if (Keyboard.FocusedElement == null) return false;

                Type focusedElement = Keyboard.FocusedElement.GetType();

                if (focusedElement.IsSubclassOf(typeof(TextBoxBase))) return true;
            }
            catch (Exception e)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), e.Message);
            }

            return false;
        }
    }
}
