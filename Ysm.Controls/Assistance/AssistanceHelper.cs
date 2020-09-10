using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Ysm.Core.KeyboadrHook;

namespace Ysm.Controls
{

    //! NOTE: Assistance nem tartalmazhat másik assitance control-t.
    public class AssistanceHelper
    {
        private readonly KeyboardHook _keyboardHook;

        private readonly Window _mainWindow;

        private readonly string _userGuide;

        private AssistanceWindow _assistanceWindow;

        private Assistance _assistance;

        private string _document;

        private bool _stopped;

        public AssistanceHelper(Window mainWindow, string userGuide)
        {
            _mainWindow = mainWindow;
            _userGuide = userGuide;

            _keyboardHook = new KeyboardHook();
            _keyboardHook.KeyDown += KeyDown;

            EventManager.RegisterClassHandler(typeof(Assistance), UIElement.MouseEnterEvent, new RoutedEventHandler(Assistance_MouseEnter));
        }

        private void Assistance_MouseEnter(object sender, RoutedEventArgs e)
        {
            if (sender is Assistance assistance)
            {
                if (_stopped == false)
                {
                    _assistance?.Deactivate();
                }

                _assistance = assistance;
                _document = assistance.Document;

                if (_stopped == false && _assistanceWindow != null)
                {
                    _assistance.Activate();
                    _assistanceWindow.ShowDocument(_document);
                }
            }
        }

        private void KeyDown(object sender, KeyHookEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                CreateWindow();
            }
            if (e.Key == Key.Escape)
            {
                DestroyWindow();
            }
            if (e.Key == Key.Space)
            {
                StopStart();
            }
        }

        private void StopStart()
        {
            if(_assistanceWindow == null) return;

            if (_stopped)
            {
                _stopped = false;
                
                Deactivate(_mainWindow);

                _assistance.Activate();
                _assistanceWindow.ShowDocument(_document);

            }
            else
            {
                _stopped = true;
            }
        }

        private void DestroyWindow()
        {
            if (_assistanceWindow != null)
            {
                _assistanceWindow.Close();
                _assistanceWindow = null;
            }
        }

        public void CreateWindow()
        {
            // Az F1-re csak akkor reagálok ha a főablak aktív.
            if (_mainWindow.IsActive == false) return;

            if (_assistanceWindow == null)
            {
                _assistanceWindow = new AssistanceWindow(_userGuide);
                _assistanceWindow.Owner = _mainWindow;
                _assistanceWindow.Closed += Window_Closed;
                _assistanceWindow.Loaded += Window_Loaded;
                _assistanceWindow.Show();
            }
            else
            {
                _assistanceWindow?.Activate();
            }
        }

        private void Deactivate(DependencyObject obj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);

                if (child is Assistance assistance)
                {
                    assistance.Deactivate();

                }
                else
                {
                    Deactivate(child);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _assistance?.Activate();
            _assistanceWindow?.ShowDocument(_document);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _assistanceWindow.Closed -= Window_Closed;
            _assistanceWindow.Loaded -= Window_Loaded;
            _assistanceWindow = null;

            _assistance?.Deactivate();
        }

        public void Dispose()
        {
            _assistanceWindow?.Close();
            _keyboardHook?.Dispose();
        }
    }
}
