using System.Windows;
using System.Windows.Input;
using Ysm.Assets;
using Ysm.Core;
using Ysm.Data;
using Ysm.Views;

namespace Ysm.Windows
{
    public partial class HistorySettings
    {
        public HistorySettings()
        {
            InitializeComponent();
        }

        private void HistorySettings_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.History)
            {
                HistoryState.Text = Properties.Resources.Button_PauseHistory;
            }
            else
            {
                HistoryState.Text = Properties.Resources.Button_TurnOnHistory;
            }
        }

        private void ClearHistory_OnClick(object sender, MouseButtonEventArgs e)
        {
            Repository.Default.History.RemoveAll();

            ViewRepository.Get<HistoryView>()?.Items.Clear();
        }

        private void PauseHistory_OnClick(object sender, MouseButtonEventArgs e)
        {
            if (Settings.Default.History)
            {
                Settings.Default.History = false;
                HistoryState.Text = Properties.Resources.Button_TurnOnHistory;
            }
            else
            {
                Settings.Default.History = true;
                HistoryState.Text = Properties.Resources.Button_PauseHistory;
            }
        }
    }
}
