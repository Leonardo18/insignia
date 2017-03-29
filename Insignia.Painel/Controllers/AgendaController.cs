using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Insignia.Painel.Helpers.CustomAttributes;
using Insignia.Painel.Helpers.Google;
using System;
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
            CalendarService service = OAuthService.Handle(Convert.ToString(Session["UsuarioID"]),
                "", "http://localhost:53966/Agenda/Visualizar",
                "Calendar API",
                new[] { CalendarService.Scope.CalendarReadonly });

            return View();
        }

        /// <summary>
        /// POST: Agenda Visualizar
        /// </summary>
        /// <returns>Retorna a view de visualizar agenda</returns>
        [HttpPost, IsLogged]
        public ActionResult Visualizar(int i = 0)
        {
            CalendarService service = OAuthService.Handle(Convert.ToString(Session["UsuarioID"]),
                "", "http://localhost:53966/Agenda/Visualizar",
                "Calendar API",
                new[] { CalendarService.Scope.CalendarReadonly });

            //DataResource.RealtimeResource.GetRequest request = service.Data.Realtime.Get(String.Format("ga:{0}", profileId), "rt:activeUsers");
            //RealtimeData feed = request.Execute();

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();

            //Upcoming events
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    string when = eventItem.Start.DateTime.ToString();
                    if (string.IsNullOrEmpty(when))
                    {
                        when = eventItem.Start.Date;
                    }
                }
            }

            return View();
        }

        /// <summary>
        /// POST: Agenda Visualizar
        /// </summary>
        /// <returns>Retorna a view de visualizar agenda</returns>
        [HttpPost, IsLogged]
        public ActionResult Authorize()
        {
            return View();
        }
    }
}