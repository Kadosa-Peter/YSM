using System;
using System.Diagnostics;
using System.Reflection;
using Ysm.Assets;
using Ysm.Core;
using Ysm.Data;

namespace Ysm.Actions
{
    public class DownloadVideo : IAction
    {
        public string Name { get; } = "DownloadVideo";

        public void Execute(object obj)
        {
            try
            {
                if(obj == null)
                {
                    if (Kernel.Default.SelectedVideoItem != null)
                    {
                        Video video = Kernel.Default.SelectedVideoItem.Video;
                        Process.Start("Ysm.Downloader.exe", video.VideoId);
                    }
                }
                else
                {
                    string id = obj as string;
                    Process.Start("Ysm.Downloader.exe", id);
                }
            }
            catch (Exception e)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), e.Message);
            }
        }
    }
}
