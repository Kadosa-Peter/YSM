using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace Ysm.Core
{
    public static partial class ExtensionMethods
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotNull(this string @this)
        {
            if (string.IsNullOrEmpty(@this))
                return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull(this string @this)
        {
            if (string.IsNullOrEmpty(@this))
                return true;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string InsertEnd(this string @this, string str)
        {
            return @this.Insert(@this.Length, str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Remove(this string @this, string str)
        {
            return @this.Replace(str, "");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Clear(this string @this)
        {
            return Regex.Replace(@this, @"\s+", " ");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;

            if (value.Length > maxLength)
            {
                value = value.Substring(0, maxLength);
                value += "...";
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this string @this, string str, bool toLower)
        {
            if (toLower)
            {
                return @this.ToLower().Contains(str);
            }

            return @this.Contains(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLetterOrNumber(this string input)
        {
            return Regex.IsMatch(input, @"^[a-zA-Z0-9]+$");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SecureString ConvertToSecureString(this string str)
        {
            SecureString secureStr = new SecureString();
            if (str.Length > 0)
            {
                foreach (char c in str) secureStr.AppendChar(c);
            }
            return secureStr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConvertToString(this SecureString secureStr)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureStr);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ToBool(this string str)
        {
            if (bool.TryParse(str, out bool value))
            {
                return value;
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime ToDateTime(this string str)
        {
            if (DateTime.TryParse(str, out DateTime value))
            {
                return value;
            }

            return default(DateTime);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this string @this)
        {
            if (int.TryParse(@this, out int i))
            {
                return i;
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToSqlDateTime(this DateTime @this)
        {
            return @this.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SolidColorBrush ToSolidColorBrush(this string str)
        {
            object obj = ColorConverter.ConvertFromString(str);
            if (obj != null)
                return new SolidColorBrush((Color)obj);

            return default(SolidColorBrush);
        }

        public static Color ToColor(this string str)
        {
            object obj = ColorConverter.ConvertFromString(str);
            if (obj != null)
                return (Color)obj;

            return default(Color);
        }
    }
}
