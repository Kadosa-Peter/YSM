using System.Windows.Controls;
using Ysm.Controls;

namespace Ysm.Assets.Menu
{
    public class MarkerMenu : MenuBase
    {
        public ContextMenu Get()
        {
            ExtendedContextMenuItem open = new ExtendedContextMenuItem();
            open.Title = Properties.Resources.Button_Open;
            open.CommandName = "OpenCommand";
            Menu.Items.Add(open);

            ExtendedContextMenuItem openTab = new ExtendedContextMenuItem();
            openTab.Title = Properties.Resources.Button_OpenTab;
            openTab.CommandName = "OpenTabCommand";
            Menu.Items.Add(openTab);

            ExtendedContextMenuItem edit = new ExtendedContextMenuItem();
            edit.Title = Properties.Resources.Button_Edit;
            edit.CommandName = "RenameCommand";
            Menu.Items.Add(edit);

            ExtendedContextMenuItem remove = new ExtendedContextMenuItem();
            remove.Title = Properties.Resources.Button_Remove;
            remove.CommandName = "RemoveCommand";
            Menu.Items.Add(remove);

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
