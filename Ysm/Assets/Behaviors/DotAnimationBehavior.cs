using System;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace Ysm.Assets.Behaviors
{
    class DotAnimationBehavior : Behavior<TextBlock>
    {
        private TextBlock _textBlock;
        private int _index;
        
        protected override void OnAttached()
        {
            base.OnAttached();

            _textBlock = AssociatedObject;
            _textBlock.MinWidth = 10;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            switch (_index)
            {
                case 0:
                {
                    _textBlock.Text = string.Empty;
                    _index++;
                }
                    break;
                case 1:
                {
                    _textBlock.Text = ".";
                    _index++;
                }
                    break;
                case 2:
                {
                    _textBlock.Text = "..";
                    _index++;
                }
                    break;
                case 3:
                {
                    _textBlock.Text = "...";
                    _index = 0;
                }
                    break;
            }
        }
    }
}
