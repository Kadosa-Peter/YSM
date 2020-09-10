using System.Windows;
using System.Windows.Input;

namespace Ysm.Controls
{
    public static class WindowExtension
    {
        public static void InitializeWindow(this Window window)
        {
            WindowCommandHelper helper = new WindowCommandHelper { Window = window };

            window.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, helper.OnSystemCloseWindowCommand));
            window.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, helper.OnSystemMaximizeWindowCommand));
            window.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, helper.OnSystemMinimizeWindowCommand));
            window.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, helper.OnSystemRestoreWindowCommand));
        }
    }

}
