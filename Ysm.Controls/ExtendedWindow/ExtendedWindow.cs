using System;
using System.Windows;

namespace Ysm.Controls
{
    public class ExtendedWindow : Window
    {
        #region ShowMaxButton

        public static readonly DependencyProperty ShowMaxButtonProperty = DependencyProperty.Register(
            "ShowMaxButton", typeof(bool), typeof(ExtendedWindow), new PropertyMetadata(default(bool)));

        public bool ShowMaxButton
        {
            get => (bool)GetValue(ShowMaxButtonProperty);
            set => SetValue(ShowMaxButtonProperty, value);
        }

        #endregion

        #region ShowRestoreButton

        public static readonly DependencyProperty ShowRestoreButtonProperty = DependencyProperty.Register(
            "ShowRestoreButton", typeof(bool), typeof(ExtendedWindow), new PropertyMetadata(default(bool)));

        public bool ShowRestoreButton
        {
            get => (bool)GetValue(ShowRestoreButtonProperty);
            set => SetValue(ShowRestoreButtonProperty, value);
        }

        #endregion

       
        public ExtendedWindow()
        {
            this.InitializeWindow();

            SetWindowStateButtons();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            SetWindowStateButtons();
        }

        private void SetWindowStateButtons()
        {
            if (WindowState == WindowState.Maximized)
            {
                ShowRestoreButton = true;
                ShowMaxButton = false;
            }
            else
            {
                ShowRestoreButton = false;
                ShowMaxButton = true;
            }
        }
    }
}
