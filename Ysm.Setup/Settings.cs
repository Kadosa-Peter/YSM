using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Ysm.Setup
{
    public static class Settings
    {
        public static void Delete()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string path = Path.Combine(appData, "Ysm", "v1");

            foreach (FileInfo file in new DirectoryInfo(path).GetFiles("*json", SearchOption.AllDirectories))
            {
                if (file.Name == "Settings.json")
                {
                    using (StreamReader reader = new StreamReader(file.FullName, Encoding.UTF8))
                    {
                        string json = reader.ReadToEnd();

                        string version = GetFirstInstance<string>("Version", json);

                        if (version == "1.0")
                        {
                            file.Delete();
                        }
                    }
                }
            }
        }

        // https://stackoverflow.com/a/19438910
        public static T GetFirstInstance<T>(string propertyName, string json)
        {
            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.PropertyName
                        && (string)jsonReader.Value == propertyName)
                    {
                        jsonReader.Read();

                        var serializer = new JsonSerializer();
                        return serializer.Deserialize<T>(jsonReader);
                    }
                }
                return default(T);
            }
        }
    }
}
