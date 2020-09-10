using System.IO;
using System.Text;
using Microsoft.WindowsAPICodePack.Shell;
using Ysm.Core;

namespace Ysm.Downloader.Assets
{
    public static class DownloadPath
    {
        public static string Get()
        {
            string output;

            if (File.Exists(FileSystem.Downloads))
            {
                string dir;

                using (StreamReader reader = new StreamReader(FileSystem.Downloads, Encoding.UTF8))
                {
                    dir = reader.ReadToEnd();
                }

                if (Directory.Exists(dir))
                {
                    output = dir;
                }
                else
                {
                    output = KnownFolders.Downloads.Path;
                }
            }
            else
            {
                output = KnownFolders.Downloads.Path;
            }

            return output;
        }
    }
}
