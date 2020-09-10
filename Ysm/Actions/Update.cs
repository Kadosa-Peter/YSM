using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Ysm.Assets;
using Ysm.Core;
using Ysm.Core.Update;
using Ysm.Windows;

namespace Ysm.Actions
{
    public class Update : IAction
    {
        public string Name { get; } = "Update";

        public void Execute(object obj)
        {
            Task<bool> task = Task.Run(() => CheckUpdate.Check());

            task.ContinueWith(t =>
            {
                if (t.Result)
                {
                    string path = Path.Combine(FileSystem.Startup, "Ysm.Update.exe");

                    if (File.Exists(path))
                    {
                        Process.Start(path);
                    }
                }
                else
                {
                    bool showInfoWindow = (bool)obj;

                    if (showInfoWindow)
                    {
                        InfoWindow window = new InfoWindow(Properties.Resources.About_Update1);
                        window.ShowDialog();
                    }
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
