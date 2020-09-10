using System.Windows;
using Ysm.Assets;
using Ysm.Windows;

namespace Ysm.Actions
{
    public class OpenSettings : IAction
    {
        public string Name { get; } = "OpenSettings";

        private SettingsWindow _settingsWindow;
        private bool _settingsWindowIsOpen;

        public void Execute(object obj)
        {
            if (!_settingsWindowIsOpen)
            {
                _settingsWindow = new SettingsWindow
                {
                    Owner = Application.Current.MainWindow
                };

                _settingsWindow.Loaded += (k, l) => { _settingsWindowIsOpen = true; };
                _settingsWindow.Closed += (n, m) =>
                {
                    _settingsWindowIsOpen = false;

                    Settings.Default.Save();
                };

                _settingsWindow.ShowDialog();
            }
            else
            {
                _settingsWindow.Activate();
            }
        }
    }
}
