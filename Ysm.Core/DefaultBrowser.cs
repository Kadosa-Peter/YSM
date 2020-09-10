using Microsoft.Win32;

namespace Ysm.Core
{
    public static class DefaultBrowser
    {
        public static string Get()
        {
            const string userChoice = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice";

            string browser;

            using (RegistryKey userChoiceKey = Registry.CurrentUser.OpenSubKey(userChoice))
            {
                object progIdValue = userChoiceKey?.GetValue("Progid");

                if (progIdValue == null)
                {
                    return "Unknown";
                }

                var progId = progIdValue.ToString();

                switch (progId)
                {
                    case "IE.HTTP":
                        browser = "IE";
                        break;
                    case "appxq0fevzme2pys62n3e0fbqa7peapykr8v":
                        browser = "Edge";
                        break;
                    case "FirefoxURL":
                        browser = "Firefox";
                        break;
                    case "ChromeHTML":
                        browser = "Chrome";
                        break;
                    case "VivaldiHTM.X7XX3D53RN2A4N7AGWQBWILPAQ":
                        browser = "Vivaldi";
                        break;
                    default:
                        browser = "Unknown";
                        break;
                }
            }

            return browser;
        }

    }
}
