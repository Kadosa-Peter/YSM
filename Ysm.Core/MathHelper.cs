using System;

namespace Ysm.Core
{
    public static class MathHelper
    {
        public static double TruncateDecimal(double value, int precision)
        {
            double step = Math.Pow(10, precision);
            double tmp = Math.Truncate(step * value);
            return tmp / step;
        }
    }
}
