using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Ysm.Assets;
using Ysm.Core;

namespace Ysm.Actions
{
    public class DownloadAllVideos : IAction
    {
        public string Name { get; } = nameof(DownloadAllVideos);

        public void Execute(object obj)
        {
            try
            {
                List<string> ids = obj as List<string>;

                StringBuilder builder = new StringBuilder();

                builder.Append("@");

                int index = 0;

                foreach (string id in ids)
                {
                    builder.Append(id);
                    builder.Append(",");

                    if(++index == 500) break;
                }

                Process.Start("Ysm.Downloader.exe", builder.ToString());
            }
            catch (Exception e)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), e.Message);
            }
        }
    }
}
