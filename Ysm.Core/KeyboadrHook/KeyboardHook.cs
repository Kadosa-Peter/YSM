using System;
using System.Reflection;
using System.Windows.Input;

namespace Ysm.Core.KeyboadrHook
{
    public class KeyboardHook
    {
        public EventHandler<KeyHookEventArgs> KeyDown;
        public EventHandler<KeyHookEventArgs> KeyUp;

        private readonly InterceptKeys _interceptKeys = new InterceptKeys();

        private bool _alt;
        private bool _ctrl;
        private bool _shift;

        private Key _lastKey;

        public KeyboardHook()
        {
            _interceptKeys.Install();
            _interceptKeys.KeyDown += InterceptKeysKeyDown;
            _interceptKeys.KeyUp += InterceptKeysKeyUp;
        }

        private void InterceptKeysKeyDown(VKeys vkey)
        {
            try
            {
                Key key = KeyInterop.KeyFromVirtualKey((int)vkey);

                bool isModifier = IsModifierDown(vkey);

                if (KeyDown != null && key != _lastKey)
                {
                    _lastKey = key;

                    KeyHookEventArgs args = new KeyHookEventArgs
                    {
                        Alt = _alt,
                        Shift = _shift,
                        Ctrl = _ctrl,
                        IsModifier = isModifier,
                        Key = key
                    };

                    KeyDown?.Invoke(this, args);
                }
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = GetType().Assembly.FullName,
                    ClassName = GetType().FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion
            }
        }

        private void InterceptKeysKeyUp(VKeys vkey)
        {
            try
            {
                Key key = KeyInterop.KeyFromVirtualKey((int)vkey);

                bool isModifier = IsModifierUp(vkey);

                _lastKey = default(Key);

                if (KeyUp != null)
                {
                    KeyHookEventArgs args = new KeyHookEventArgs
                    {
                        Alt = _alt,
                        Shift = _shift,
                        Ctrl = _ctrl,
                        IsModifier = isModifier,
                        Key = key
                    };

                    KeyUp?.Invoke(this, args);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private bool IsModifierUp(VKeys vkey)
        {
            bool isModifier = false;

            if (vkey == VKeys.LMENU || vkey == VKeys.LMENU)
            {
                _alt = false;
                isModifier = true;
            }
            if (vkey == VKeys.LCONTROL || vkey == VKeys.RCONTROL)
            {
                _ctrl = false;
                isModifier = true;
            }
            if (vkey == VKeys.LSHIFT || vkey == VKeys.RSHIFT)
            {
                _shift = false;
                isModifier = true;
            }

            return isModifier;
        }

        private bool IsModifierDown(VKeys vkey)
        {
            bool isModifier = false;

            if (vkey == VKeys.LMENU || vkey == VKeys.LMENU)
            {
                _alt = true;
                isModifier = true;
            }
            if (vkey == VKeys.LCONTROL || vkey == VKeys.RCONTROL)
            {
                _ctrl = true;
                isModifier = true;
            }
            if (vkey == VKeys.LSHIFT || vkey == VKeys.RSHIFT)
            {
                _shift = true;
                isModifier = true;
            }

            return isModifier;
        }

        public void Dispose()
        {
            _interceptKeys.Uninstall();
        }
    }
}