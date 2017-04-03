using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Insignia.DAO.Autenticacao.Google;
using Insignia.Painel.ViewModels;
using Insignia.Painel.Helpers.CustomAttributes;
using System;
using System.Configuration;
using System.Web.Mvc;
using Insignia.Model.Agenda;
using System.Collections.Generic;

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
            var ViewModel = new ViewModelAgenda();

            CalendarService service = OAuthService.Service
                                (
                                    Convert.ToString(Session["UsuarioID"]),
                                    ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString, "http://localhost:53966/Agenda/SincronizarAgenda",
                                    "Calendar API",
                                    new[] { CalendarService.Scope.CalendarReadonly, CalendarService.Scope.Calendar }
                                );

            if (service != null)
            {
                //Define os parâmetros do request.
                EventsResource.ListRequest request = service.Events.List("primary");
                //request.TimeMin = DateTime.Now;
                request.ShowDeleted = false;
                request.SingleEvents = true;                
                //request.MaxResults = 10;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                //Lista de eventos.
                Events events = request.Execute();

                //Eventos que irão acontecer
                if (events.Items != null && events.Items.Count > 0)
                {
                    ViewModel.ListAgenda = new List<Agenda>();

                    foreach (var eventItem in events.Items)
                    {
                        ViewModel.ListAgenda.Add(new Agenda()
                        {
                            Titulo = eventItem.Summary,
                            DataInicio = Convert.ToDateTime(eventItem.Start.DateTime),
                            DataFim = Convert.ToDateTime(eventItem.End.DateTime)
                        });

                        string EventoData = Convert.ToString(eventItem.Start.DateTime);
                        string EventoDescricao = eventItem.Summary;
                        if (string.IsNullOrEmpty(EventoData))
                        {
                            EventoData = eventItem.Start.Date;
                        }
                    }
                }               
            }

            return View(ViewModel);
        }

        /// <summary>
        /// POST: Agenda Sincronizar Agenda
        /// </summary>
        /// <returns>Sincroniza agenda com o google</returns>
        [HttpGet, IsLogged]
        public ActionResult SincronizarAgenda()
        {
            var ViewModel = new ViewModelAgenda();

            CalendarService service = OAuthService.OAuthLogin
                                (
                                    Convert.ToString(Session["UsuarioID"]),
                                    ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString, "http://localhost:53966/Agenda/SincronizarAgenda",
                                    "Calendar API",
                                    new[] { CalendarService.Scope.CalendarReadonly, CalendarService.Scope.Calendar }
                                );

            if (service != null)
            {
                //Define os parâmetros do request.
                EventsResource.ListRequest request = service.Events.List("primary");
                //request.TimeMin = DateTime.Now;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                //request.MaxResults = 10;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                //Lista de eventos.
                Events events = request.Execute();

                //Eventos que irão acontecer
                if (events.Items != null && events.Items.Count > 0)
                {
                    ViewModel.ListAgenda = new List<Agenda>();

                    foreach (var eventItem in events.Items)
                    {
                        ViewModel.ListAgenda.Add(new Agenda()
                        {
                            Titulo = eventItem.Summary,
                            DataInicio = Convert.ToDateTime(eventItem.Start.DateTime),
                            DataFim = Convert.ToDateTime(eventItem.End.DateTime)
                        });

                        string EventoData = Convert.ToString(eventItem.Start.DateTime);
                        string EventoDescricao = eventItem.Summary;
                        if (string.IsNullOrEmpty(EventoData))
                        {
                            EventoData = eventItem.Start.Date;
                        }
                    }
                }              
            }

            return View("Visualizar", ViewModel);
        }
    }
}