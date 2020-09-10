using System;
using System.Windows.Input;

namespace Ysm.Core.KeyboadrHook
{
    public class KeyHookEventArgs : EventArgs
    {
        public bool Shift { get; set; }

        public bool Alt { get; set; }

        public bool Ctrl { get; set; }

        public bool Win { get; set; }

        public bool IsModifier { get; set; }

        public Key Key { get; set; }
    }
}