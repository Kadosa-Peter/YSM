using System.Windows;
using Ysm.Assets;
using Ysm.Windows;

namespace Ysm.Actions
{
    public class OpenAbout : IAction
    {
        public string Name { get; } = "OpenAbout";

        public void Execute(object obj)
        {
            AboutWindow window = new AboutWindow();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }
    }
}
