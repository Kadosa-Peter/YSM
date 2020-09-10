using System;
using System.Windows;
using Ysm.Controls;

namespace Ysm.Test
{
    public partial class MainWindow
    {

        public MainWindow()
        {
            InitializeComponent();

            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
        }

       
    }
}
