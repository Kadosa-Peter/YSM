using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ysm.Downloader.Assets
{
    public class BooleanToCollapsedConverterInvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (bool) value)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
