using System;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using Google.Apis.Oauth2.v2;
using Newtonsoft.Json;
using Ysm.Core;

namespace Ysm.Data
{
    public class AuthenticatioHelper
    {
        public static string GetAutenticationUri(SecureString clientId, string redirectUri)
        {
            string scopes = $"{Scopes.YouTube} {Scopes.Profile} {Scopes.Email} ";

            string oauth = string.Format("https://accounts.google.com/o/oauth2/auth?client_id={0}&redirect_uri={1}&scope={2}&response_type=code", clientId.ConvertToString(), redirectUri, scopes);

            return new Uri(oauth).AbsoluteUri;
        }

        public static Token GetTokenResponse(string authCode, SecureString clientid, SecureString secret, string redirectUri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://accounts.google.com/o/oauth2/token");

            string postData = $"code={authCode}&client_id={clientid.ConvertToString()}&client_secret={secret.ConvertToString()}&redirect_uri={redirectUri}&grant_type=authorization_code";

            byte[] data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            Token tokenToken = PharseResponseString(responseString);

            return tokenToken;
        }

        private static Token PharseResponseString(string responseString)
        {
            Token tokenToken = JsonConvert.DeserializeObject<Token>(responseString);
            tokenToken.Created = DateTime.Now;
            return tokenToken;
        }

        public static bool IsUserExists(string id)
        {
            string userDir = Path.Combine(FileSystem.AppData, id);

            return Directory.Exists(userDir);
        }

        public static User CreateUser(Userinfoplus userInfo, Token token)
        {
            User user = new User
            {
                Id = userInfo.Id,
                Name = userInfo.Name,
                Email = userInfo.Email,
                Picture = userInfo.Picture
            };

            string userDir = Path.Combine(FileSystem.AppData, userInfo.Id);

            FileSystem.User = userDir;

            // create user directory
            Directory.CreateDirectory(userDir);

            // save user
            Path.Combine(userDir, "User").WriteText(JsonConvert.SerializeObject(user));

            // save token
            Path.Combine(userDir, "Token").WriteText(JsonConvert.SerializeObject(token));

            // save current user
            FileSystem.CurrentUser.WriteText(userInfo.Id);

            return user;
        }

        public static User SetUser(string userId)
        {
            string userDir = Path.Combine(FileSystem.AppData, userId);

            FileSystem.CurrentUser.WriteText(userId);

            User user = JsonConvert.DeserializeObject<User>(Path.Combine(userDir, "User").ReadText());

            FileSystem.User = userDir;

            return user;
        }

        public static User GetCurrentUser()
        {
            string currentUser = FileSystem.CurrentUser;

            if (File.Exists(currentUser))
            {
                string userId = currentUser.ReadText();

                string userDir = Path.Combine(FileSystem.AppData, userId);

                FileSystem.User = userDir;

                if (Directory.Exists(userDir))
                {
                    string user = Path.Combine(userDir, "User");

                    if (File.Exists(user))
                    {
                        return JsonConvert.DeserializeObject<User>(Path.Combine(userDir, "User").ReadText());
                    }
                }
            }

            return null;
        }

        public static Token GetCurrentUserToken(string userId)
        {
            string userDir = Path.Combine(FileSystem.AppData, userId);

            if (Directory.Exists(userDir))
            {
                string token = Path.Combine(userDir, "Token");

                if (File.Exists(token))
                {
                    return JsonConvert.DeserializeObject<Token>(Path.Combine(token).ReadText());
                }
            }

            return null;
        }

        public static void SaveUser(User user)
        {
            string userDir = Path.Combine(FileSystem.AppData, user.Id);

            Path.Combine(userDir, "User").WriteText(JsonConvert.SerializeObject(user));
        }
    }
}
