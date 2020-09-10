using System;
using System.Windows;
using System.Windows.Interactivity;
using Ysm.Core;

namespace Ysm.Downloader.Assets.Behaviors
{
    public class DropBehavior : Behavior<MainWindow>
    {
        private MainWindow _window;

        protected override void OnAttached()
        {
            base.OnAttached();

            _window = AssociatedObject;
            _window.PreviewDrop += MainWindow_OnPreviewDrop;
        }

        private void MainWindow_OnPreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                string data = (string)e.Data.GetData(DataFormats.StringFormat);

                data = Validator.ValidateVideoId(data);

                if (data.NotNull())
                {
                    _window.CreateDownload(data);
                }
                else
                {
                    DialogHelper.ShowInfoWindow("Invalid video id or url!");
                }

                e.Handled = true;
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] data = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (data != null)
                {
                    foreach (string file in data)
                    {
                        if (file.EndsWith("url", StringComparison.OrdinalIgnoreCase))
                        {
                            string url = UrlHelper.GetUrl(file);

                            string id = Validator.ValidateVideoId(url);

                            if (id.NotNull())
                            {
                                _window.CreateDownload(id);
                            }
                        }
                    }
                }

                e.Handled = true;
            }
        }
    }
}
