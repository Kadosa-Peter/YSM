using System;
using System.Globalization;
using System.Windows.Data;

namespace Ysm.Core
{
    public class TimeSpanFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan ts = (TimeSpan) value;

            if (ts.Hours > 0)
            {
                return ts.ToString(@"h\:mm\:ss");
            }
            else
            {
                return ts.ToString(@"mm\:ss");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
