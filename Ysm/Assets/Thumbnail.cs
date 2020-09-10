using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Ysm.Core;

namespace Ysm.Assets
{

    public class Thumbnail : Image
    {
        #region Url

        public static readonly DependencyProperty UrlProperty = DependencyProperty.Register(
            "Url", typeof(string), typeof(Thumbnail), new PropertyMetadata(default(string)));

        public string Url
        {
            get => (string)GetValue(UrlProperty);
            set => SetValue(UrlProperty, value);
        }

        #endregion

        #region Unavailable

        public static readonly DependencyProperty UnavailableProperty = DependencyProperty.Register(
            "Unavailable", typeof(bool), typeof(Thumbnail), new PropertyMetadata(default(bool)));

        public bool Unavailable
        {
            get => (bool) GetValue(UnavailableProperty);
            set => SetValue(UnavailableProperty, value);
        }

        #endregion

        private BitmapImage _image;

        public Thumbnail()
        {
            ImageFailed += Thumbnail_ImageFailed;
            Loaded += Thumbnail_Loaded;
        }

        private void Thumbnail_Loaded(object sender, RoutedEventArgs e)
        {
            if (Url.NotNull())
            {
                _image = new BitmapImage();
                _image.BeginInit();
                _image.UriSource = new Uri(Url, UriKind.Absolute);
                _image.CacheOption = BitmapCacheOption.OnLoad;
                _image.DecodePixelWidth = 240;
                _image.DecodePixelHeight = 135;
                _image.EndInit();

                Source = _image;
            }
        }

        private void Thumbnail_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            _image = new BitmapImage();
            _image.BeginInit();
            _image.UriSource = new Uri("/Ysm;component/Resources/default.png", UriKind.Relative);
            _image.DecodePixelWidth = 240;
            _image.DecodePixelHeight = 135;
            _image.EndInit();

            Source = _image;

            Unavailable = true;
        }

      
    }
}
