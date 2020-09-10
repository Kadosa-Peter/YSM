using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;

namespace Ysm.Core
{
    public static class ColorHelper
    {
        private static List<Color> _colors;

        private static Color _defaultFolderColor;

        public static IReadOnlyList<Color> GetKnownColors()
        {
            if (_colors == null)
            {
                _colors = new List<Color>();

                Type ColorType = typeof(Colors);
                PropertyInfo[] arrPiColors = ColorType.GetProperties(BindingFlags.Public | BindingFlags.Static);

                foreach (PropertyInfo pi in arrPiColors)
                    _colors.Add((Color)pi.GetValue(null, null));
                return _colors;
            }
            else
            {
                return _colors;
            }
        }

        public static Color GetRandomColor()
        {
            var list = GetKnownColors();
            Random rnd = new Random(DateTime.Now.Ticks.GetHashCode());
            return list[rnd.Next(list.Count)];
        }

        public static Color  GetDefaultFolderColor()
        {
            if (_defaultFolderColor == default(Color))
            {
                _defaultFolderColor = "#DCB679".ToColor();
            }

            return _defaultFolderColor;
        }

    }
}
