using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Threading;
using System.Web;

namespace Insignia.Painel.Helpers.Google
{
    public static class OAuthService
    {
        /// <summary>
        /// Authenticate to Google Using Oauth2
        /// Documentation https://developers.google.com/accounts/docs/OAuth2
        /// </summary>
        /// <param name="clientId">From Google Developer console https://console.developers.google.com</param>
        /// <param name="clientSecret">From Google Developer console https://console.developers.google.com</param>
        /// <param name="userName">A string used to identify a user.</param>
        /// <returns></returns>
        public static CalendarService AuthenticateOauth(string clientId, string clientSecret, string userName)
        {

            string[] scopes = new string[]
            {
                CalendarService.Scope.Calendar  ,  // Manage your calendars
                CalendarService.Scope.CalendarReadonly    // View your Calendars
            };

            try
            {
                // here is where we Request the user to give us access, or use the Refresh Token that was previously stored in %AppData%
                UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets { ClientId = "215187720738-qvd9a4kbm69cqd5iuutgekhspg67l8ar.apps.googleusercontent.com", ClientSecret = "96JWX7tgheXLn1pe5QJw968E" }
                                                                    , scopes
                                                                    , "Insígnia"
                                                                    , CancellationToken.None
                                                                    , new FileDataStore(HttpContext.Current.Server.MapPath("~/Content/uploads/.credentials"))).Result;


                // Create Google Calendar API service.
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Calendar API",
                });

                return service;
            }
            catch (Exception ex)
            {

                HttpContext.Current.Response.Write(ex.Message);
                HttpContext.Current.Response.End();
                return null;

            }
        }
    }
}