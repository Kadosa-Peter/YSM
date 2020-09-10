using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Ysm.Core;
using Ysm.Data;
using Ysm.Views;

namespace Ysm.Windows
{
    public partial class UnwatchedWindow
    {
        public UnwatchedWindow()
        {
            InitializeComponent();
        }

        private void UnwatchedWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Increase_OnClick(object sender, RoutedEventArgs e)
        {
            int day = int.Parse(Day.Text);

            if (day < 60)
            {
                day++;
                Day.Text = day.ToString();
            }

            PluralSingular.Text = $"{Properties.Resources.Text_Days}.";
        }

        private void Decrease_OnClick(object sender, RoutedEventArgs e)
        {
            int day = int.Parse(Day.Text);

            if (day > 1)
            {
                day--;
                Day.Text = day.ToString();
            }

            if (day == 1)
            {
                PluralSingular.Text = $"{Properties.Resources.Text_Day}.";
            }
            else
            {
                PluralSingular.Text = $"{Properties.Resources.Text_Days}.";
            }
        }

        private void MarkUnwatched_OnClick(object sender, RoutedEventArgs e)
        {
            int day = int.Parse(Day.Text);
            day = day * -1;

            DateTime dateTime = DateTime.Now.AddDays(day);

            Task.Run(() => { Repository.Default.Videos.MarkUnwatchedAfter(dateTime); }).ContinueWith(t =>
            {
                ViewRepository.Get<ChannelView>().Reset();
                ViewRepository.Get<VideoView>().Reset();
                ViewRepository.Get<FooterView>().UpdateCount();
                Close();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
