using System.Windows;
using System.Windows.Input;

namespace Ysm.Controls
{
    public class WindowCommandHelper
    {
        public Window Window { get; set; }

        public void OnSystemCloseWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(Window);
        }

        public void OnSystemMinimizeWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(Window);
        }

        public void OnSystemMaximizeWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(Window);
        }

        public void OnSystemRestoreWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(Window);
        }
    }
}
