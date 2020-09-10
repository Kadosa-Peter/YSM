using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;

namespace Ysm.Setup
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var args = Environment.GetCommandLineArgs();

            if (args.Any(x => x == "uninstall"))
            {
                Ngen.UnInstall();
            }
            if (args.Any(x => x == "install"))
            {
                //Settings.Delete();

                EAS("Ysm.exe");
                EAS("Ysm.Feedback.exe");

                DeleteCache();

                Database.AddColorColumn();
                Database.AddContinueTable();

                Ngen.Install();
            }

            Close();
        }

        private static void DeleteCache()
        {
            try
            {
                DirectoryInfo info = new DirectoryInfo(@"cef\Resources\cache");

                foreach (DirectoryInfo directory in info.GetDirectories())
                {
                    directory.Delete(true);
                }

                foreach (FileInfo file in info.GetFiles())
                {
                    file.Delete();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        private static void EAS(string file)
        {
            if (File.Exists($"{file}.config"))
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(file);
                    ConfigurationSection section = config.GetSection("appSettings");
                    if (section != null && section.SectionInformation.IsProtected == false)
                    {
                        if (!section.SectionInformation.IsProtected)
                        {
                            if (!section.ElementInformation.IsLocked)
                            {
                                section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                                section.SectionInformation.ForceSave = true;
                                config.Save(ConfigurationSaveMode.Full);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

        }
    }
}
