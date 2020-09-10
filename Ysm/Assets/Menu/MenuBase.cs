using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Ysm.Controls;
using Ysm.Core;

namespace Ysm.Assets.Menu
{
    public abstract class MenuBase
    {
        public event EventHandler OnMenuOpened;

        public event EventHandler OnMenuClosed;

        public readonly ExtendedContextMenu Menu;

        public readonly Settings ApplicationSettings;

        protected MenuBase()
        {
            Menu = new ExtendedContextMenu();
            Menu.Opened += Menu_Opened;
            Menu.Closed += Menu_Closed;

            ApplicationSettings = Settings.Default;
            ApplicationSettings.PropertyChanged += ApplicationSettings_PropertyChanged;
        }

        private void Menu_Closed(object sender, RoutedEventArgs e)
        {
            Task.Delay(2000).GetAwaiter().OnCompleted(() => Kernel.Default.MenuIsOpen = false);

            OnMenuClosed?.Invoke(this, e);

            MenuClosing();
        }

        private void Menu_Opened(object sender, RoutedEventArgs e)
        {
            Kernel.Default.MenuIsOpen = true;

            OnMenuOpened?.Invoke(this, e);

            MenuOpening();
        }

        public virtual void MenuOpening()
        {

        }

        public virtual void MenuClosing()
        {

        }

        private void ApplicationSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ApplicationSettingsChanged(e.PropertyName);
        }

        public virtual void ApplicationSettingsChanged(string propertyName)
        {

        }

        public ExtendedContextMenuItem CreateSeparator(string id = null)
        {
            return new ExtendedContextMenuItem
            {
                IsSeparator = true,
                Style = (Style)Application.Current.FindResource("SeparatorStyle"),
                Id = id
            };
        }

        public void SetCommands()
        {
            foreach (ExtendedContextMenuItem item in Menu.Items)
            {
                SetCommand(item);

                if (item.HasItems)
                {
                    foreach (ExtendedContextMenuItem subItem in item.Items)
                    {
                        SetCommand(subItem);
                    }
                }
            }
        }

        public void SetCommand(ExtendedContextMenuItem item)
        {
            if (item.CommandName.NotNull())
            {
                Binding binding = new Binding
                {
                    Source = CommandsHelper.Commands,
                    Path = new PropertyPath(item.CommandName)
                };

                item.SetBinding(MenuItem.CommandProperty, binding);
            }
        }

        public ExtendedContextMenuItem Find(string id)
        {
            foreach (ExtendedContextMenuItem item in Menu.Items)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
