using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using RestSharp;
using RestSharp.Authenticators;
using Ysm.Core;

namespace Ysm.Feedback
{
    public partial class MainWindow
    {
        private SecureString _mailgun;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = GetType().Assembly.FullName,
                    ClassName = GetType().FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Configuration configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ConfigurationSection appSettings = configFile.GetSection("appSettings");

                if (appSettings != null && appSettings.SectionInformation.IsProtected == false)
                {
                    appSettings.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                    appSettings.SectionInformation.ForceSave = true;
                    configFile.Save(ConfigurationSaveMode.Modified);
                }

                _mailgun = ConfigurationManager.AppSettings["Mailgun"].ConvertToSecureString();

                tbMessage.ForceFocus();
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = GetType().Assembly.FullName,
                    ClassName = GetType().FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            btnSubmit.IsEnabled = false;

            string topic = GetTopic();
            string message = GetMessage();

            try
            {
                RestClient client = new RestClient
                {
                    BaseUrl = new Uri("https://api.mailgun.net/v3"),
                    Authenticator = new HttpBasicAuthenticator("api", _mailgun.ConvertToString())
                };

                RestRequest request = new RestRequest();
                request.AddParameter("domain", "yosuma.com", ParameterType.UrlSegment);
                request.Resource = "{domain}/messages";
                request.AddParameter("from", "info@yosuma.com");
                request.AddParameter("to", "info@yosuma.com");
                request.AddParameter("subject", topic);

                if (File.Exists(tbAttachment.Text))
                    request.AddFile("attachment", tbAttachment.Text);

                request.AddParameter("text", message);

                request.Method = Method.POST;

                client.ExecuteAsync(request, response =>
                {
                    MailSent();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MailSent()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                CloseWindow window = new CloseWindow();
                window.Show();

                Close();
            }));
        }

        private string GetTopic()
        {
            if (btnIdea.IsChecked == true)
            {
                return "Idea";
            }
            else if (btnProblem.IsChecked == true)
            {
                return "Problem";
            }
            else
            {
                return "Question";
            }
        }

        private string GetMessage()
        {
            string message = tbMessage.Text;

            if (tbEmail.Text.NotNull())
            {
                string email = $"From {tbEmail.Text}";

                message = $"{email}{Environment.NewLine}{Environment.NewLine}{message}";
            }


            return message;
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Text.IsNull())
            {
                btnSubmit.IsEnabled = false;
            }
            else
            {
                btnSubmit.IsEnabled = true;
            }
        }

        private void Footer_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void OpenFileBrowser_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                Multiselect = false,
                Title = "Screenshot - only image",
                EnsureReadOnly = true,
                Filters = { new CommonFileDialogFilter("image", "*.jpg, *.jpeg, *.png, *.gif, *.tiff, *.tif, *.bmp") },
                DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                IsFolderPicker = false,
                AllowNonFileSystemItems = false
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string fileName = dialog.FileName;

                bool result = CheckFileName(fileName);

                if (result)
                {
                    tbAttachment.Text = fileName;
                }
            }


        }

        private bool CheckFileName(string fileName)
        {
            FileInfo info = new FileInfo(fileName);

            if (info.Length > 5000000)
            {
                MessageBox.Show("The attachment file size must be less than 5MB.");

                return false;
            }

            return fileName.EndsWith(".jpg")
                   || fileName.EndsWith(".jpeg")
                   || fileName.EndsWith(".png")
                   || fileName.EndsWith(".gif")
                   || fileName.EndsWith(".tiff")
                   || fileName.EndsWith(".tif")
                   || fileName.EndsWith(".bmp");
        }
    }
}