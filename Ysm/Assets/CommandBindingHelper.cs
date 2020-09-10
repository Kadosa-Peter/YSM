namespace Ysm.Assets
{
    public class CommandBindingHelper
    {
        public Commands Commands => _commands ?? (_commands = new Commands());
        private Commands _commands;
    }
}
