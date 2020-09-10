using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace Ysm.Core
{
    public static class DesignMode
    {
        private static bool? _isInDesignMode;

        public static bool IsInDesignMode()
        {
            if (!_isInDesignMode.HasValue)
            {
                DependencyProperty prop = DesignerProperties.IsInDesignModeProperty;

                _isInDesignMode = (bool)DependencyPropertyDescriptor
                    .FromProperty(prop, typeof(FrameworkElement))
                    .Metadata.DefaultValue;

                // Just to be sure
                if (!_isInDesignMode.Value && Process.GetCurrentProcess().ProcessName.StartsWith("devenv", StringComparison.Ordinal))
                {
                    _isInDesignMode = true;
                }
            }

            return _isInDesignMode.Value;
        }

        public static bool IsInDesignMode(DependencyObject obj)
        {
            return DesignerProperties.GetIsInDesignMode(obj);
        }
    }
}
