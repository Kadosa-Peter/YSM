using Ysm.Data;

namespace Ysm.Windows
{
    public partial class VideoDragWindow
    {
        public VideoDragWindow(Video video)
        {
            InitializeComponent();

            Thumbnail.Url = video.ThumbnailUrl;
        }
    }
}
