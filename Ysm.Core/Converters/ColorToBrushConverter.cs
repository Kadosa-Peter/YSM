using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Ysm.Core
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return new SolidColorBrush((Color)value);

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ReSharper disable once PossibleNullReferenceException
            Color color = ((SolidColorBrush)value).Color;

            return color;
        }
    }
}
