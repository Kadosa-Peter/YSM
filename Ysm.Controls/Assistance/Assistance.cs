using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Ysm.Controls
{
    /// <summary>
    /// Abban az esetben, ha az animáció nem látható, a processzort nem terheli.
    /// ~300 példány körülbelül 5MB-tal növeli a memóriát.
    /// </summary>
    public class Assistance : ContentControl
    {

        #region IsActive
       
        // Ha meg van nyitva a AssistanceWindow akkor aktív.
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(Assistance), new PropertyMetadata(default(bool), IsActive_PropertyChanged));

        [Category("Common")]
        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        private static void IsActive_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Assistance assistance = d as Assistance;
            bool value = (bool)e.NewValue;

            if (assistance != null && assistance.IsEnabled && assistance.Visibility == Visibility.Visible)
            {
                if (value)
                {
                    assistance.ShowFocusBorder = true;
                }
                else
                {
                    assistance.ShowFocusBorder = false;
                }
            }
        }


        #endregion

        #region ShowFocusBorder

        // Ha az egeret a Control főlé visszük és a AssistanceWindow meg van nyitva (IsActive = true), akkor megjelenítem a „Assistance-Indikátor-Bigyót”.
        public static readonly DependencyProperty ShowFocusBorderProperty = DependencyProperty.Register
            ("ShowFocusBorder", typeof(bool), typeof(Assistance), new PropertyMetadata(default(bool)));

        [Category("Common")]
        public bool ShowFocusBorder
        {
            get => (bool)GetValue(ShowFocusBorderProperty);
            set => SetValue(ShowFocusBorderProperty, value);
        }
        #endregion

        #region Document

        // Az Document (id) alapján keresem meg a megfelelő súgó-dokumentumot. 
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register
            ("Document", typeof(string), typeof(Assistance), new PropertyMetadata(default(string)));

        [Category("Common")]
        public string Document
        {
            get => (string)GetValue(DocumentProperty);
            set => SetValue(DocumentProperty, value);
        }  
        #endregion

        // Static .ctor
        static Assistance()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Assistance), new FrameworkPropertyMetadata(typeof(Assistance)));
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

    }
}
