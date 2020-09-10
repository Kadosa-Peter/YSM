using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace Ysm.Core
{
    public class CurrentScreen
    {
        private readonly Screen _screen;

        public bool IsPrimary => _screen.Primary;

        public string DeviceName => _screen.DeviceName;

        public CurrentScreen(Screen screen)
        {
            _screen = screen;
        }

        public Rect DeviceBounds => GetRect(_screen.Bounds);

        public Rect WorkingArea => GetRect(_screen.WorkingArea);

        private Rect GetRect(Rectangle value)
        {
            // should x, y, width, height be device-independent-pixels ??
            return new Rect
            {
                X = value.X,
                Y = value.Y,
                Width = value.Width,
                Height = value.Height
            };
        }
    }
}
