using System;
using System.Globalization;
using System.Windows.Data;

namespace Ysm.Assets.Converters
{
    class StateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                int i = (int) value;

                return i > 0;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;

            if (b)
                return 1;

            return 0;
        }
    }
}
