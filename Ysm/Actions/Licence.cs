using System.Windows;
using Ysm.Assets;
using Ysm.Core;
using Ysm.Views;
using Ysm.Windows;

namespace Ysm.Actions
{
    public class Licence : IAction
    {
        public string Name { get; } = "Licence";

        public void Execute(object obj)
        {
            if (Assets.Licence.IsValid() == false)
            {
                SerialWindow window = new SerialWindow(false);
                window.Owner = Application.Current.MainWindow;
                window.ShowDialog();

                if (Assets.Licence.IsValid())
                {
                    ViewRepository.Get<HeaderView>()?.HideTrial();
                }
            }
        }
    }
}
