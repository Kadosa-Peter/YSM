using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Ysm.Core
{
    public class RandomColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush(ColorHelper.GetRandomColor());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
