using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Ysm.Core;

namespace Ysm.Data
{
    public class AuthenticationService
    {
        #region Singleton

        private static readonly Lazy<AuthenticationService> Instance = new Lazy<AuthenticationService>(() => new AuthenticationService());

        public static AuthenticationService Default => Instance.Value;

        private AuthenticationService()
        {
            User = AuthenticatioHelper.GetCurrentUser();
        }

        #endregion

        public YouTubeService YouTubeService
        {
            get
            {
                if (_youTubeService == null && User != null)
                {
                    Token token = AuthenticatioHelper.GetCurrentUserToken(User.Id);

                    SetupYoutubeService(null, token);
                }

                return _youTubeService;
            }
        }
        private YouTubeService _youTubeService;

        public event EventHandler LoggedIn;
        public event EventHandler LoggedOut;
        public event EventHandler Shutdown;

        public User User { get; set; }

        public bool IsLoggedIn => User != null;

        public SecureString ClientId { get; set; }

        public SecureString ClientSecret { get; set; }

        public void Initialize()
        {
            if (_youTubeService == null && User != null)
            {
                Token token = AuthenticatioHelper.GetCurrentUserToken(User.Id);

                SetupYoutubeService(null, token);
            }
        }

        public async void CreateUser(string authCode, string redirectUri)
        {
            try
            {
                Token tokenToken = AuthenticatioHelper.GetTokenResponse(authCode, ClientId, ClientSecret, redirectUri);

                Userinfoplus userInfo = await GetUserInfo(tokenToken);

                if (AuthenticatioHelper.IsUserExists(userInfo.Id))
                {
                    // set existing user
                    User = AuthenticatioHelper.SetUser(userInfo.Id);
                }
                else
                {
                    // create new user
                    User = AuthenticatioHelper.CreateUser(userInfo, tokenToken);
                }

                await Application.Current.Dispatcher.BeginInvoke(new Action(() => LoggedIn?.Invoke(this, EventArgs.Empty)));
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

        private async Task<Userinfoplus> GetUserInfo(Token tokenToken)
        {
            UserCredential credential = Credential.GetUserCredential(tokenToken, ClientId, ClientSecret);

            SetupYoutubeService(credential, tokenToken);

            Oauth2Service oauthSerivce = new Oauth2Service(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = GetType().ToString()
                });

            Userinfoplus userInfo = await oauthSerivce.Userinfo.Get().ExecuteAsync();

            return userInfo;
        }

        private void SetupYoutubeService(UserCredential credential, Token token)
        {
            try
            {
                if (credential == null)
                {
                    credential = Credential.GetUserCredential(token, ClientId, ClientSecret);
                }

                _youTubeService = new YouTubeService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = GetType().ToString()
                });
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

        public void Delete()
        {
            User = null;

            LoggedOut?.Invoke(this, EventArgs.Empty);

            Task.Delay(2000).GetAwaiter().OnCompleted(() =>
            {
                try
                {
                    if (Directory.Exists(FileSystem.User))
                    {
                        Directory.Delete(FileSystem.User, true);
                    }

                    if (File.Exists(FileSystem.CurrentUser))
                    {
                        File.Delete(FileSystem.CurrentUser);
                    }

                    FileInfo file = new DirectoryInfo(FileSystem.AppData).GetFiles().Find("Google.Apis.Auth.OAuth2.Responses");

                    file?.Delete();

                    Shutdown?.Invoke(this, EventArgs.Empty);
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
                        Trace = ex.StackTrace,
                    };

                    Logger.Log(error);

                    #endregion
                }
            });
        }

        public void Logout()
        {
            if (File.Exists(FileSystem.CurrentUser))
            {
                File.Delete(FileSystem.CurrentUser);
            }

            User = null;

            LoggedOut?.Invoke(this, EventArgs.Empty);
        }
    }
}
