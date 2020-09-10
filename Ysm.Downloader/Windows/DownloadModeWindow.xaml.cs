using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using YoutubeExplode.Models;
using Ysm.Controls;
using Ysm.Downloader.Assets;

namespace Ysm.Downloader.Windows
{
    public partial class DownloadModeWindow
    {
        public PlaylistDownloadMode PlaylistDownloadMode { get; set; }

        public int PreferredQuality { get; set; }

        public bool IsCancelled { get; set; }

        public DownloadModeWindow(Playlist playlist)
        {
            InitializeComponent();

            int count = playlist.Videos.Count;
            string text = count > 1 ? Properties.Resources.Text_Video : Properties.Resources.Text_Videos;
            Header.Text = $"{playlist.Title} - {count} {text}";
        }

        public DownloadModeWindow(int count)
        {
            InitializeComponent();

            if (count == 1)
            {
                Header.Text = $"{count} {Properties.Resources.Text_Video}";
            }
            else
            {
                Header.Text = $"{count} {Properties.Resources.Text_Videos}";
            }
        }

        private void Download_OnClick(object sender, RoutedEventArgs e)
        {
            if (DownloadOneByOne.IsChecked == true)
            {
                PlaylistDownloadMode = PlaylistDownloadMode.DownloadOneByOne;
            }
            else
            {
                PlaylistDownloadMode = PlaylistDownloadMode.DownloadAllAtOnce;

                foreach (object obj in QualityPanel.Children)
                {
                    if (obj is RadioButton button && button.IsChecked == true)
                    {
                        PreferredQuality = Convert.ToInt32(button.Tag);
                    }
                }
            }

            Close();
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            IsCancelled = true;

            Close();
        }

        private void Header_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Footer_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
