using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Insignia.DAO.Autenticacao.Google;
using Insignia.DAO.Competencias;
using Insignia.DAO.Tarefas;
using Insignia.DAO.Usuarios;
using Insignia.Model.Agenda;
using Insignia.Painel.Helpers.CustomAttributes;
using Insignia.Painel.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class DashboardController : Controller
    {
        /// <summary>
        /// GET: Empresa
        /// </summary>
        /// <returns>Retorna a view do dashboard da empresa</returns>
        [HttpGet, IsLogged]
        public ActionResult Empresa()
        {
            return View();
        }

        /// <summary>
        /// GET: Gestor
        /// </summary>
        /// <returns>Retorna a view do dashboard do gestor</returns>
        [HttpGet, IsLogged]
        public ActionResult Gestor()
        {
            return View();
        }

        /// <summary>
        /// GET: Funcionario
        /// </summary>
        /// <returns>Retorna a view do dashboard do funcionário</returns>
        [HttpGet, IsLogged]
        public ActionResult Funcionario()
        {
            var ViewModel = new ViewModelDashboardFuncionario();

            UsuariosDAO UsuariosDAO = new UsuariosDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

            ViewModel.Usuario = UsuariosDAO.Carregar(Convert.ToInt32(Session["UsuarioID"]));

            TarefasDAO TarefasDAO = new TarefasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);
            //Busca as tarefa com status finalizada
            ViewModel.ListFinalizadas = TarefasDAO.ListarTop(ConfigurationManager.AppSettings["Fazer"], 0, 3);

            CompetenciasDAO CompetenciasDAO = new CompetenciasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

            ViewModel.ListCompetencias = CompetenciasDAO.Listar();

            foreach (var item in ViewModel.ListCompetencias)
            {
                item.Pontos = CompetenciasDAO.CompetenciaPontos(item.ID);
            }

            CalendarService service = OAuthService.OAuthLogged
                               (
                                   Convert.ToString(Session["UsuarioID"]),
                                   ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString, "https://www.portalinsignia.com.br/Dashboard/SincronizarAgenda",
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
                        ViewModel.IconeRefreshCor = "green";
                    }                    
                }
            }
            else
            {
                ViewModel.IconeRefreshCor = "red";
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
                                    ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString, "https://www.portalinsignia.com.br/Dashboard/SincronizarAgenda",
                                    "Calendar API",
                                    new[] { CalendarService.Scope.CalendarReadonly, CalendarService.Scope.Calendar }
                                );

            return RedirectToAction("Funcionario");
        }
    }
}