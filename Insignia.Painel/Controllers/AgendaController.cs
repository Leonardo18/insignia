using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Insignia.DAO.Autenticacao.Google;
using Insignia.Painel.Helpers.CustomAttributes;
using System;
using System.Configuration;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class AgendaController : Controller
    {
        /// <summary>
        /// GET: Agenda Visualizar
        /// </summary>
        /// <returns>Retorna a view de visualizar agenda</returns>
        [HttpGet, IsLogged]
        public ActionResult Visualizar()
        {
            CalendarService service = OAuthService.OAuthLogin
                                    (
                                        Convert.ToString(Session["UsuarioID"]),
                                        ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString, "http://localhost:53966/Agenda/Visualizar",
                                        "Calendar API",
                                        new[] { CalendarService.Scope.CalendarReadonly }
                                    );
            return View();
        }

        /// <summary>
        /// POST: Agenda Visualizar
        /// </summary>
        /// <returns>Retorna a view de visualizar agenda</returns>
        [HttpPost, IsLogged]
        public ActionResult Visualizar(int i = 0)
        {
            CalendarService service = OAuthService.OAuthLogin
                                    (
                                        Convert.ToString(Session["UsuarioID"]),
                                        ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString, "http://localhost:53966/Agenda/Visualizar",
                                        "Calendar API",
                                        new[] { CalendarService.Scope.CalendarReadonly }
                                    );

            //Define os parâmetros do request.
            EventsResource.ListRequest request = service.Events.List("primary");
            //request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            //Lista de eventos.
            Events events = request.Execute();

            //Eventos que irão acontecer
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    string EventoData = Convert.ToString(eventItem.Start.DateTime);
                    string EventoDescricao = eventItem.Summary;
                    if (string.IsNullOrEmpty(EventoData))
                    {                        
                        EventoData = eventItem.Start.Date;
                    }
                }
            }

            return View();
        }
    }
}