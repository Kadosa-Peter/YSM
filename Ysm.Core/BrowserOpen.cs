using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Ysm.Core
{
    public static class BrowserOpen
    {
        public static void OpenUrl(string url)
        {
            if (url.NotNull())
            {
                try
                {
                    Process.Start(url);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        public static void OpenUrls(IEnumerable<string> urls)
        {
            string browser = DefaultBrowser.Get();

            int milliseconds = 0;

            List<Action> actions = new List<Action>();

            foreach (string url in urls)
            {
                var local = milliseconds;

                actions.Add(() => { Task.Delay(local).GetAwaiter().OnCompleted(() => Process.Start(url)); });

                milliseconds += 300;

                if (milliseconds == 600 && IsProcessRunning(browser) == false)
                {
                    milliseconds = 1500;
                }
            }

            try
            {
                actions.ForEach(x => x.Invoke());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);   
            }
        }

        private static bool IsProcessRunning(string name)
        {
            return Process.GetProcessesByName(name).Any();
        }
    }
}
