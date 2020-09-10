using System;
using System.IO;
using System.Threading.Tasks;
using Chromium;
using Ysm.Core;
using Ysm.Data;

namespace Ysm.Assets.Browser
{
    public class HtmlSource
    {
        public static void SaveHtmlSource(string htmlSource, long frameId)
        {
            Video video = Kernel.Default.PlayerVideo;

            string channel = ChannelMapper.Get(video.ChannelId).Title;

            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Html Source", channel);

            folder = Files.VerifyFolderPath(folder);

            if (Directory.Exists(folder) == false)
                Directory.CreateDirectory(folder);

            string file = $"{video.VideoId} - {frameId} - {DateTime.Now:T}.txt";

            file = file.Replace(":", "_");

            file = Path.Combine(folder, file);

            file = Files.VerifyFilePath(file);

            file.WriteText(htmlSource);
        }

        public static Task<string> GetSourceAsync(CfxFrame frame)
        {
            TaskStringVisitor taskStringVisitor = new TaskStringVisitor();
            frame.GetSource(taskStringVisitor);

            return taskStringVisitor.Task;
        }
    }
}
