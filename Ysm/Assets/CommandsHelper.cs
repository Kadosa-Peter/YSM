using System.Windows;

namespace Ysm.Assets
{
    public static class CommandsHelper
    {
        public static Commands Commands
        {
            get
            {
                if (_commands == null)
                {
                    CommandBindingHelper helper = Application.Current.FindResource("CommandBindingHelper") as CommandBindingHelper;
                    _commands = helper?.Commands;
                }

                return _commands;
            }
        }

        private static Commands _commands;
    }
}
