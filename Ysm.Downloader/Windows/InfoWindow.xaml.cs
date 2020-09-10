using System;
using System.Windows;
using System.Windows.Input;
using Ysm.Core;

namespace Ysm.Downloader.Windows
{
    public partial class InfoWindow
    {
        /// <summary>
        /// W: 400
        /// H: 180
        /// </summary>
        public InfoWindow(string info)
        {
            InitializeComponent();

            if (Owner == null)
                WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (info.NotNull())
            {
                if (info.Contains(@"\n"))
                {
                    info = info.Replace(@"\n", Environment.NewLine);
                }

                Info.Text = info;
            }
        }

        public InfoWindow()
        {
            InitializeComponent();
        }

        private void InfoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DoneButton.ForceFocus();
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void InfoWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            if (e.Key == Key.Enter)
            {
                Close();
            }
        }

        private void Footer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
