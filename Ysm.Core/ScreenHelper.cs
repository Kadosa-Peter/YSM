using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Ysm.Core
{
    public static class ScreenHelper
    {
        public static CurrentScreen Primary => new CurrentScreen(Screen.PrimaryScreen);

        public static CurrentScreen GetScreen(Window window)
        {
            WindowInteropHelper windowInteropHelper = new WindowInteropHelper(window);
            Screen screen = Screen.FromHandle(windowInteropHelper.Handle);
            CurrentScreen currentScreen = new CurrentScreen(screen);
            return currentScreen;
        }

        public static CurrentScreen GetScreen(Point point)
        {
            int x = (int)Math.Round(point.X);
            int y = (int)Math.Round(point.Y);

            // x,y device-independent-pixels ?
            System.Drawing.Point drawingPoint = new System.Drawing.Point(x, y);
            Screen screen = Screen.FromPoint(drawingPoint);
            CurrentScreen currentScreen = new CurrentScreen(screen);

            return currentScreen;
        }

        public static IEnumerable<CurrentScreen> AllScreens()
        {
            return Screen.AllScreens.Select(screen => new CurrentScreen(screen));
        }
    }
}
