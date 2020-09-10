using System;
using System.Collections.Generic;
using System.Reflection;
using Chromium.Remote.Event;
using Chromium.WebBrowser;
using Ysm.Core;

namespace Ysm.Assets.Browser
{
    class BoundObject : JSObject
    {
        readonly Dictionary<string, Action<object>> _actions = new Dictionary<string, Action<object>>();

        public void Add(Action<object> func, string function)
        {
            _actions.Add(function, func);

            AddFunction(function).Execute += Execute;
        }

        private void Execute(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            try
            {
                Action<object> action = _actions[e.Name];

                action?.Invoke(e.Arguments);
            }
            catch (Exception ex)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), ex.Message);
            }
        }
    }
}
