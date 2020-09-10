using System.Windows.Controls;
using Ysm.Controls;

namespace Ysm.Assets.Menu
{
    public class WatchLaterNodeMenu : MenuBase
    {
        public ContextMenu Get()
        {
            ExtendedContextMenuItem remove = new ExtendedContextMenuItem();
            remove.Title = Properties.Resources.Button_Remove;
            remove.Hotkey = "Del";
            remove.CommandName = "WatchLaterRemoveChannelCommand";
            Menu.Items.Add(remove);

            ExtendedContextMenuItem removeAll = new ExtendedContextMenuItem();
            removeAll.Title = Properties.Resources.Button_RemoveAll;
            removeAll.CommandName = "WatchLaterRemoveAllCommand";
            Menu.Items.Add(removeAll);

            //Menu.Items.Add(CreateSeparator());

            //ExtendedContextMenuItem openSettings = new ExtendedContextMenuItem();
            //openSettings.Title = Properties.Resources.Button_Settings;
            //openSettings.Hotkey = "CTRL + K";
            //openSettings.CommandName = "SettingsCommand";
            //Menu.Items.Add(openSettings);

            //ExtendedContextMenuItem help = new ExtendedContextMenuItem();
            //help.Title = Properties.Resources.Button_Help;
            //help.Hotkey = "F1";
            //help.CommandName = "HelpCommand";
            //Menu.Items.Add(help);

            //ExtendedContextMenuItem close = new ExtendedContextMenuItem();
            //close.Title = Properties.Resources.Button_Close;
            //close.CommandName = "CloseCommand";
            //Menu.Items.Add(close);

            SetCommands();

            return Menu;
        }
        
    }
}
