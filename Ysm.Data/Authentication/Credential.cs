using System;
using System.Reflection;
using System.Security;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Ysm.Core;

namespace Ysm.Data
{
    internal static class Credential
    {
        internal static UserCredential GetUserCredential(Token token, SecureString clientId, SecureString clientSecret)
        {
            try
            {
                string[] scopes = { Scopes.YouTube, Scopes.Profile, Scopes.Email };

                GoogleAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = clientId.ConvertToString(),
                        ClientSecret = clientSecret.ConvertToString()
                    },
                    Scopes = scopes,
                    DataStore = new FileDataStore(FileSystem.AppData)
                });

                TokenResponse tokenResponse = new TokenResponse
                {
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken 
                };

                UserCredential credential = new UserCredential(flow, Environment.UserName, tokenResponse);

                return credential;
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(Credential).Assembly.FullName,
                    ClassName = typeof(Credential).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion

                throw;
            }
        }
    }
}
