using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace Ysm.Feedback
{
    public partial class CloseWindow
    {
        public CloseWindow()
        {
            InitializeComponent();
        }

        private void Background_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void CloseWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Timer timer = new Timer(5000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(Close));
        }

       
    }
}
