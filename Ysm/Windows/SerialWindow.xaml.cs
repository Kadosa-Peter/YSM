using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Ysm.Assets;
using Ysm.Core;

namespace Ysm.Windows
{
    public partial class SerialWindow : INotifyPropertyChanged
    {
        public string Serial
        {
            get => _serial;

            set
            {
                if (_serial != value)
                {
                    _serial = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _serial;

        public bool IsInvalid
        {
            get => _isInvalid;

            set
            {
                if (_isInvalid != value)
                {
                    _isInvalid = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isInvalid;

        private int _attempt;

        public SerialWindow(bool hideTitle = true)
        {
            InitializeComponent();

            DataContext = this;

            //if (DebugMode.IsDebugMode)
            //{
            //    Serial = "";
            //}

            if (hideTitle == false)
            {
                SerialWindowTitle.Visibility = Visibility.Collapsed;
            }
        }

        private void SerialWindow2_OnActivated(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                string serial = Clipboard.GetText(TextDataFormat.Text);

                serial = serial.Trim();

                if (serial.Length == 19 && serial.Count(f => f == '-') == 3)
                {
                    Serial = serial;
                }
            }
        }

        private void Validate_OnClick(object sender, RoutedEventArgs e)
        {
            _attempt++;

            bool valid = CheckUserCode();

            if (valid)
            {
                Settings.Default.Validated = StringEncryption.Encrypt("Validated", Kernel.Default.EncryptionKey.ConvertToString());
                Settings.Default.Save();
                Close();
            }
            else
            {
                IsInvalid = true;

                if (_attempt > 1)
                {
                    InvalidProductKey.Text = $"({_attempt}) {Properties.Resources.Title_InvalidProductKey}";
                }
            }
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool CheckUserCode()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Kernel.Default.ConnectionString.ConvertToString()))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT [Id] FROM [Users] WHERE [Serial] =  '{Serial}' AND IsActive=1";

                        object obj = command.ExecuteScalar();

                        if (obj != null)
                            return true;
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void StoreLink_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://www.yosuma.com");
        }
    }
}
