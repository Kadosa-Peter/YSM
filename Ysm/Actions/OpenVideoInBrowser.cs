using System.Diagnostics;
using Ysm.Assets;
using Ysm.Core;

namespace Ysm.Actions
{
    public class OpenVideoInBrowser : IAction
    {
        public string Name { get; } = "OpenVideoInBrowser";

        public void Execute(object obj)
        {
            if (obj is string id)
            {
                string url = UrlHelper.GetVideoUrl(id);

                Process.Start(url);
            }
        }
    }
}
