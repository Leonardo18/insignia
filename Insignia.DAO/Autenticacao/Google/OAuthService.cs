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
        /// Efetua login OAuth no google e grava dados em banco de dados para não precisar ficar logando a cada acesso
        /// </summary>
        /// <param name="usuarioID">ID do usuário que está logando</param>
        /// <param name="conexaoBanco">String de conexão com banco de dados</param>
        /// <param name="urlRedirecionamento">URL que o google irá redirecionar de volta após o Login</param>
        /// <param name="aplicacaoNome">Nome da aplicação no google console developer</param>
        /// <param name="escopos">Escopos que definem o que será usado do serviço</param>
        /// <returns>Retorna o serviço que será usado</returns>
        public static CalendarService OAuthLogin(string usuarioID, string conexaoBanco, string urlRedirecionamento, string aplicacaoNome, string[] escopos)
        {
            try
            {
                CalendarService service = new CalendarService();

                //Use uma classe extendida para autenticação Google Flow
                GoogleAuthorizationCodeFlow flow;
                flow = new ForceOfflineGoogleAuthorizationCodeFlow(
                    new GoogleAuthorizationCodeFlow.Initializer
                    {
                        //Classe GoogleDAO para salvar o token de acesso no banco de dados.
                        DataStore = new GoogleDAO(conexaoBanco),
                        ClientSecrets = new ClientSecrets { ClientId = "215187720738-qvd9a4kbm69cqd5iuutgekhspg67l8ar.apps.googleusercontent.com", ClientSecret = "96JWX7tgheXLn1pe5QJw968E" },
                        Scopes = escopos
                    });


                var uri = Convert.ToString(HttpContext.Current.Request.Url);

                //URL de redirecionamento configurada no painel do google developers console.
                string uriRedirecionamento = urlRedirecionamento;

                if (HttpContext.Current.Request["code"] != null)
                {
                    var token = flow.ExchangeCodeForTokenAsync(usuarioID, HttpContext.Current.Request["code"], uri.Substring(0, uri.IndexOf("?")), CancellationToken.None).Result;

                    // Extrai dados salvos.
                    var oauthState = AuthWebUtility.ExtracRedirectFromState(flow.DataStore, usuarioID, HttpContext.Current.Request["state"]).Result;

                    var dados = new AuthorizationCodeWebApp(flow, uriRedirecionamento, uri).AuthorizeAsync(usuarioID, CancellationToken.None).Result;

                    //Caso já exista no banco de dados o token, o usuário já possui permissão e está logado.
                    service = new CalendarService
                    (
                        new BaseClientService.Initializer()
                        {
                            HttpClientInitializer = dados.Credential,
                            ApplicationName = aplicacaoNome
                        }
                    );

                    return service;
                }
                else
                {
                    var dados = new AuthorizationCodeWebApp(flow, uriRedirecionamento, uri).AuthorizeAsync(usuarioID, CancellationToken.None).Result;

                    if (dados.RedirectUri != null)
                    {
                        //Redireciona o usuário para fazer login e dar as permissões
                        HttpContext.Current.Response.Redirect(dados.RedirectUri);
                    }
                    else
                    {
                        //Caso já exista no banco de dados o token, o usuário já possui permissão e está logado.
                        service = new CalendarService
                        (
                            new BaseClientService.Initializer()
                            {
                                HttpClientInitializer = dados.Credential,
                                ApplicationName = aplicacaoNome
                            }
                        );

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
        /// Verifica se o usuário está logado no Google Calendar
        /// </summary>
        /// <param name="usuarioID">ID do usuário no sistema</param>
        /// <param name="conexaoBanco">Conexão com banco de dados</param>
        /// <param name="urlRedirecionamento">Url de redirecionamento definida no Google Console Developers</param>
        /// <param name="aplicacaoNome">Nome da aplicação no google</param>
        /// <param name="escopos">Acessos que o sistema irá solicitar ao usuário</param>
        /// <returns>Caso esteja logado retorna o serviço para ser usado, se não retorna null</returns>
        public static CalendarService OAuthLogged(string usuarioID, string conexaoBanco, string urlRedirecionamento, string aplicacaoNome, string[] escopos)
        {
            CalendarService service = new CalendarService();

            //Use uma classe extendida para autenticação Google Flow
            GoogleAuthorizationCodeFlow flow;
            flow = new ForceOfflineGoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    //Classe GoogleDAO para salvar o token de acesso no banco de dados.
                    DataStore = new GoogleDAO(conexaoBanco),
                    ClientSecrets = new ClientSecrets { ClientId = "215187720738-qvd9a4kbm69cqd5iuutgekhspg67l8ar.apps.googleusercontent.com", ClientSecret = "96JWX7tgheXLn1pe5QJw968E" },
                    Scopes = escopos
                });

            var dados = new AuthorizationCodeWebApp(flow, urlRedirecionamento, Convert.ToString(HttpContext.Current.Request.Url)).AuthorizeAsync(usuarioID, CancellationToken.None).Result;

            if (dados.RedirectUri == null)
            {
                //Caso já exista no banco de dados o token o usuário já possui permissão e está logado.
                service = new CalendarService
                (
                    new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = dados.Credential,
                        ApplicationName = aplicacaoNome
                    }
                );

                return service;
            }

            return null;
        }

        /// <summary>
        /// Faz login no google offline
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