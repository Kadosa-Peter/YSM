using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Ysm.Assets;
using Ysm.Data;

namespace Ysm.Windows
{
    public partial class UserWindow
    {
        private bool _delete;

        public UserWindow()
        {
            InitializeComponent();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UserWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            User user = AuthenticationService.Default.User;
            if (user != null)
            {
                NameText.Text = user.Name;
                EmailText.Text = user.Email;
                Picture.Source = GetUserPicture(user);
            }
        }

        private static BitmapImage GetUserPicture(User user)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(user.Picture, UriKind.Absolute);
            bitmap.EndInit();
            return bitmap;
        }

        private void Logout_OnClick(object sender, RoutedEventArgs e)
        {
            VideoServiceWrapper.Default.Cancel();
            SubscriptionServiceWrapper.Default.Cancel();

            AuthenticationService.Default.Logout();

            Close();
        }

        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            _delete = true;

            Close();
        }

        private void UserWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void UserWindow_OnClosed(object sender, EventArgs e)
        {
            if (_delete)
            {
                if (Dialogs.OpenDialog(Properties.Resources.Question_DeleteAccount))
                {
                    VideoServiceWrapper.Default.Cancel();
                    SubscriptionServiceWrapper.Default.Cancel();

                    AuthenticationService.Default.Delete();
                }
            }
        }

        private void Header_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
