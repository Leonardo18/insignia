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

            CalendarService service = OAuthService.OAuthLogged
                                (
                                    Convert.ToString(Session["UsuarioID"]),
                                    ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString, "https://www.portalinsignia.com.br/Agenda/SincronizarAgenda",
                                    "Calendar API",
                                    new[] { CalendarService.Scope.CalendarReadonly, CalendarService.Scope.Calendar }
                                );

            if (service != null)
            {
                //Cria model com as propriedades da agenda
                ViewModel.ListAgenda = new List<Agenda>();

                //Cria serviço para buscar agendas do usuário
                var ListaAgendas = service.CalendarList.List();

                //Pega todas as agendas do usuário
                var Agendas = ListaAgendas.Execute();

                //Para cada agenda busca todos os eventos
                foreach (var item in Agendas.Items)
                {
                    //Define os parâmetros do request.
                    EventsResource.ListRequest request = service.Events.List(item.Id);
                    //request.TimeMin = DateTime.Now;
                    request.ShowDeleted = false;
                    request.SingleEvents = true;
                    request.MaxResults = 10000;
                    request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                    //Lista de eventos.
                    Events events = request.Execute();

                    //Se encontrou eventos no calendário, pega os dados
                    if (events.Items != null && events.Items.Count > 0)
                    {
                        foreach (var eventItem in events.Items)
                        {
                            ViewModel.ListAgenda.Add(new Agenda()
                            {
                                Titulo = !string.IsNullOrEmpty(eventItem.Summary) ? eventItem.Summary.Replace("'", "") : "",
                                DataInicio = Convert.ToDateTime(eventItem.Start.DateTime),
                                DataFim = Convert.ToDateTime(eventItem.End.DateTime)
                            });
                        }
                    }
                }
            }

            return View(ViewModel);
        }

        /// <summary>
        /// POST: Agenda Sincronizar Agenda
        /// </summary>
        /// <returns>Sincroniza agenda com o Google</returns>
        [HttpGet, IsLogged]
        public ActionResult SincronizarAgenda()
        {
            var ViewModel = new ViewModelAgenda();

            CalendarService service = OAuthService.OAuthLogin
                                (
                                    Convert.ToString(Session["UsuarioID"]),
                                    ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString, "https://www.portalinsignia.com.br/Agenda/SincronizarAgenda",
                                    "Calendar API",
                                    new[] { CalendarService.Scope.CalendarReadonly, CalendarService.Scope.Calendar }
                                );

            if (service != null)
            {
                //Cria model com as propriedades da agenda
                ViewModel.ListAgenda = new List<Agenda>();

                //Cria serviço para buscar agendas do usuário
                var ListaAgendas = service.CalendarList.List();

                //Pega todas as agendas do usuário
                var Agendas = ListaAgendas.Execute();

                //Para cada agenda busca todos os eventos
                foreach (var item in Agendas.Items)
                {
                    //Define os parâmetros do request.
                    EventsResource.ListRequest request = service.Events.List(item.Id);
                    //request.TimeMin = DateTime.Now;
                    request.ShowDeleted = false;
                    request.SingleEvents = true;
                    request.MaxResults = 10000;
                    request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                    //Lista de eventos.
                    Events events = request.Execute();

                    //Se encontrou eventos no calendário, pega os dados
                    if (events.Items != null && events.Items.Count > 0)
                    {
                        foreach (var eventItem in events.Items)
                        {
                            ViewModel.ListAgenda.Add(new Agenda()
                            {
                                Titulo = !string.IsNullOrEmpty(eventItem.Summary) ? eventItem.Summary.Replace("'", "") : "",
                                DataInicio = Convert.ToDateTime(eventItem.Start.DateTime),
                                DataFim = Convert.ToDateTime(eventItem.End.DateTime)
                            });
                        }
                    }
                }
            }

            return View("Visualizar", ViewModel);
        }
    }
}