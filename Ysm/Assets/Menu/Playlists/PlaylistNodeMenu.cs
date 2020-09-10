using System.Windows.Controls;
using Ysm.Controls;

namespace Ysm.Assets.Menu
{
    public class PlaylistNodeMenu : MenuBase
    {
        public ContextMenu Get()
        {
            ExtendedContextMenuItem createNew = new ExtendedContextMenuItem();
            createNew.Title = Properties.Resources.Button_NewPlaylist;
            createNew.Hotkey = "F3";
            createNew.CommandName = "CreateCommand";
            Menu.Items.Add(createNew);

            ExtendedContextMenuItem rename = new ExtendedContextMenuItem();
            rename.Title = Properties.Resources.Button_Rename;
            rename.Hotkey = "F2";
            rename.CommandName = "RenameCommand";
            Menu.Items.Add(rename);

            ExtendedContextMenuItem delete = new ExtendedContextMenuItem();
            delete.Title = Properties.Resources.Button_Remove;
            delete.Hotkey = "Del";
            delete.CommandName = "RemoveCommand";
            Menu.Items.Add(delete);

            ExtendedContextMenuItem colors = new ExtendedContextMenuItem();
            colors.Title = Properties.Resources.Button_Colors;
            Menu.Items.Add(colors);

            ExtendedContextMenuItem customColors = new ExtendedContextMenuItem();
            customColors.Title = Properties.Resources.Button_CustomColors;
            customColors.CommandName = "ColorCommand";
            colors.Items.Add(customColors);

            ExtendedContextMenuItem defaultColor = new ExtendedContextMenuItem();
            defaultColor.Title = Properties.Resources.Button_DefaultColor;
            defaultColor.CommandName = "DefaultColorCommand";
            colors.Items.Add(defaultColor);

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
