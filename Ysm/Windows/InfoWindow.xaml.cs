using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Ysm.Core;

namespace Ysm.Windows
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
                    info = info.Replace(@"\n", System.Environment.NewLine);
                }

                Info1.Text = info;
            }
        }

        public InfoWindow(FlowDocument document)
        {
            InitializeComponent();

            Info1.Visibility = Visibility.Collapsed;
            Info2.Visibility = Visibility.Visible;

            if (Owner == null)
                WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Info2.Document = document;
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
