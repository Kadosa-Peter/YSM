using System;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace Ysm.Core
{
    public static class UrlHelper
    {
        public static string GetChannelUrl(string channelId)
        {
            return channelId.IsNull() ? null : $"https://www.youtube.com/channel/{channelId}";
        }

        public static string GetVideosPageUrl(string channelId)
        {
            return channelId.IsNull() ? null : $"https://www.youtube.com/channel/{channelId}/videos";
        }

        public static string GetFeedUrl(string channelId)
        {
            return channelId.IsNull() ? null : $"https://www.youtube.com/feeds/videos.xml?channel_id={channelId}";
        }

        public static BitmapImage GetIcon(string id, int width, int height)
        {
            string path = Path.Combine(FileSystem.Thumbnails, id);

            path = path.InsertEnd(".jpg");

            if (File.Exists(path))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(path, UriKind.Absolute);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.DelayCreation;
                image.DecodePixelWidth = width;
                image.DecodePixelHeight = height;
                image.EndInit();

                return image;
            }

            return DefaultIcon(width, height);
        }

        private static BitmapImage DefaultIcon(int width, int height)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("/Ysm;component/Resources/Icons/default_channel.png", UriKind.Relative);
            image.CacheOption = BitmapCacheOption.OnDemand;
            image.CreateOptions = BitmapCreateOptions.DelayCreation;
            image.DecodePixelWidth = width;
            image.DecodePixelHeight = height;
            image.EndInit();

            return image;
        }

        public static string GeUploadPlaylistId(string channelId)
        {
            StringBuilder stringBuilder = new StringBuilder(channelId)
            {
                [1] = 'U'
            };

            return stringBuilder.ToString();
        }

        public static string GetVideoUrl(string id)
        {
            return $"https://www.youtube.com/watch?v={id}";
        }
    }
}
