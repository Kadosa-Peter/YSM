using System;
using System.Globalization;
using System.Windows.Data;
using Ysm.Core;

namespace Ysm.Assets.Converters
{
    public class ChannelImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string channelId = value?.ToString();

            if (channelId != null)
            {
                return UrlHelper.GetIcon(channelId, 60, 60);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
