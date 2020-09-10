using System;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using YoutubeExplode.Models;
using YoutubeExplode.Models.MediaStreams;
using Ysm.Core;

namespace Ysm.Downloader.Assets
{
    public static class ExtensionMethods
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(this TimeSpan @this)
        {
            if (@this.Hours > 0)
            {
                return @this.ToString(@"h\:mm\:ss");
            }
            else
            {
                return @this.ToString(@"mm\:ss");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(this Container @this)
        {
            if (@this == Container.Tgpp)
            {
                return "3GPP";
            }

            return @this.ToString().ToUpper();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToMb(this long @this)
        {
            //return $"{Math.Round(Helpers.ConvertBytesToMegabytes(@this), 1)} MB";
            return Helpers.GetBytesReadable(@this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetValue(this Controls.RadioButton @this)
        {
            if (@this.IsChecked.HasValue)
            {
                return @this.IsChecked.Value;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetValue(this ComboBox @this)
        {
            return @this.SelectionBoxItem?.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetThumbnail(this Video @this)
        {
            return $"http://img.youtube.com/vi/{@this.Id}/mqdefault.jpg";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetQualityLevel(this string @this)
        {
            if (@this.IsNull() || @this.EndsWith("p") == false)
                return 0;

            return int.Parse(@this.Remove(@this.Length - 1));
        }
    }
}
