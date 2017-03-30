using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Insignia.DAO.Google;
using System;
using System.Threading;
using System.Web;

namespace Insignia.DAO.Autenticacao.Google
{
    public static class OAuthService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="usuarioID"></param>
        /// <param name="conexaoBanco"></param>
        /// <param name="urlRedirecionamento"></param>
        /// <param name="aplicacaoNome"></param>
        /// <param name="escopos"></param>
        /// <returns></returns>
        public static CalendarService Handle(string usuarioID, string conexaoBanco, string urlRedirecionamento, string aplicacaoNome, string[] escopos)
        {
            try
            {
                CalendarService service = new CalendarService();
                //Use extended class to create google authorization code flow
                GoogleAuthorizationCodeFlow flow;
                flow = new ForceOfflineGoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    //Classe GoogleDAO para salvar o token de acesso no banco de dados.
                    DataStore = new GoogleDAO(conexaoBanco),
                    ClientSecrets = new ClientSecrets { ClientId = "215187720738-qvd9a4kbm69cqd5iuutgekhspg67l8ar.apps.googleusercontent.com", ClientSecret = "96JWX7tgheXLn1pe5QJw968E" },
                    Scopes = escopos,
                });


                var uri = HttpContext.Current.Request.Url.ToString();
                string redirecturi = urlRedirecionamento;//This is the redirect URL set in google developer console.
                var code = HttpContext.Current.Request["code"];
                if (code != null)
                {
                    var token = flow.ExchangeCodeForTokenAsync(usuarioID, code,
                        uri.Substring(0, uri.IndexOf("?")), CancellationToken.None).Result;

                    var test = HttpContext.Current.Request["state"];

                    // Extract the right state.
                    var oauthState = AuthWebUtility.ExtracRedirectFromState(
                         flow.DataStore, usuarioID, HttpContext.Current.Request["state"]).Result;
                    HttpContext.Current.Response.Redirect(oauthState);
                }
                else
                {

                    var result = new AuthorizationCodeWebApp(flow, redirecturi, uri).AuthorizeAsync(usuarioID,
                         CancellationToken.None).Result;

                    if (result.RedirectUri != null)
                    {
                        // Redirect the user to the authorization server.
                        HttpContext.Current.Response.Redirect(result.RedirectUri);
                    }
                    else
                    {
                        // The data store contains the user credential, so the user has been already authenticated.
                        service = new CalendarService(new BaseClientService.Initializer()
                        {
                            HttpClientInitializer = result.Credential,
                            ApplicationName = aplicacaoNome
                        });
                        return service;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        internal class ForceOfflineGoogleAuthorizationCodeFlow : GoogleAuthorizationCodeFlow
        {
            public ForceOfflineGoogleAuthorizationCodeFlow(Initializer initializer) : base(initializer) { }

            public override AuthorizationCodeRequestUrl CreateAuthorizationCodeRequest(string redirectUri)
            {
                var ss = new GoogleAuthorizationCodeRequestUrl(new Uri(AuthorizationServerUrl));
                ss.AccessType = "offline";
                ss.ApprovalPrompt = "force";
                ss.ClientId = ClientSecrets.ClientId;
                ss.Scope = string.Join(" ", Scopes);
                ss.RedirectUri = redirectUri;
                return ss;
            }
        };
    }
}
