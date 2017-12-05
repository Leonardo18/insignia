using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Insignia.DAO.Autenticacao.Google;
using Insignia.DAO.Competencias;
using Insignia.DAO.Graficos;
using Insignia.DAO.Tarefas;
using Insignia.DAO.Usuarios;
using Insignia.Model.Agenda;
using Insignia.Painel.Helpers.CustomAttributes;
using Insignia.Painel.Helpers.Util;
using Insignia.Painel.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class DashboardController : Controller
    {
        private GraficosDAO GraficosDAO = new GraficosDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

        /// <summary>
        /// GET: Empresa
        /// </summary>
        /// <returns>Retorna a view do dashboard da empresa</returns>
        [HttpGet, IsLogged, HavePermission(AreaNome = "DashboardEmpresa")]
        public ActionResult Empresa(int FiltroSetor = 0, int FiltroUsuario = 0)
        {
            var ViewModel = new ViewModelDashboardEmpresa();

            //Análise de tarefas
            ViewModel.TarefasMes = new List<int>();

            for (int i = 1; i <= 12; i++)
            {
                ViewModel.TarefasMes.Add(GraficosDAO.QuantidadeTarefasMes(i, !string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor, FiltroUsuario));
            }

            //Análise de competências
            ViewModel.ListCompetencias = GraficosDAO.Listar();

            if (FiltroUsuario != 0)
            {
                foreach (var item in ViewModel.ListCompetencias)
                {
                    item.Pontos = GraficosDAO.CompetenciaPontos(item.ID, FiltroUsuario);
                }
            }
            else
            {
                foreach (var item in ViewModel.ListCompetencias)
                {
                    item.Pontos = 0;
                }
            }

            //Análise de badges
            ViewModel.ListBadgeBasicas = GraficosDAO.Badges("Basica", !string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor);
            ViewModel.ListBadgeIntermediarias = GraficosDAO.Badges("Intermediaria", !string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor);
            ViewModel.ListBadgeAvancadas = GraficosDAO.Badges("Avancada", !string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor);

            //Busca o total de usuário da empresa, caso o usuário logado seja gestor ou funcionário busca o total de usuários do setor correspondente
            ViewModel.TotalUsuarios = GraficosDAO.TotalUsuarios(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor);

            foreach (var item in ViewModel.ListBadgeBasicas)
            {
                ViewModel.TotalBadgesAdquiridas = GraficosDAO.BadgeAdquiridas(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor, FiltroUsuario, item.ID);

                if (ViewModel.TotalBadgesAdquiridas != 0)
                {
                    item.Adquirida = true;
                    item.Progresso = Math.Round(((double)ViewModel.TotalBadgesAdquiridas / ViewModel.TotalUsuarios) * 100, 0);
                }
                else
                {
                    if (FiltroUsuario == 0)
                    {
                        item.Adquirida = true;
                    }
                }
            }

            foreach (var item in ViewModel.ListBadgeIntermediarias)
            {
                ViewModel.TotalBadgesAdquiridas = GraficosDAO.BadgeAdquiridas(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor, FiltroUsuario, item.ID);

                if (ViewModel.TotalBadgesAdquiridas != 0)
                {
                    item.Adquirida = true;
                    item.Progresso = Math.Round(((double)ViewModel.TotalBadgesAdquiridas / ViewModel.TotalUsuarios) * 100, 0);
                }
                else
                {
                    if (FiltroUsuario == 0)
                    {
                        item.Adquirida = true;
                    }
                }
            }

            foreach (var item in ViewModel.ListBadgeAvancadas)
            {
                ViewModel.TotalBadgesAdquiridas = GraficosDAO.BadgeAdquiridas(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor, FiltroUsuario, item.ID);

                if (ViewModel.TotalBadgesAdquiridas != 0)
                {
                    item.Adquirida = true;
                    item.Progresso = Math.Round(((double)ViewModel.TotalBadgesAdquiridas / ViewModel.TotalUsuarios) * 100, 0);
                }
                else
                {
                    if (FiltroUsuario == 0)
                    {
                        item.Adquirida = true;
                    }
                }
            }

            ViewBag.UsuarioID = FiltroUsuario;

            //Recarrega o dropdownlist de setores setando o valor que havia sido usado como filtro
            var Setores = SelectListMVC.CriaListaSelecao(GraficosDAO.Setores());

            foreach (var item in Setores)
            {
                if (Convert.ToInt32(item.Value) == FiltroSetor)
                {
                    item.Selected = true;
                    break;
                }
            }

            ViewBag.Setores = Setores;

            //Recarrega o dropdownlist de usuários setando o valor que havia sido usado como filtro
            var Usuarios = SelectListMVC.CriaListaSelecao(GraficosDAO.Usuarios(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor));

            foreach (var item in Usuarios)
            {
                if (Convert.ToInt32(item.Value) == FiltroUsuario)
                {
                    item.Selected = true;
                    break;
                }
            }

            ViewBag.Usuarios = Usuarios;

            return View(ViewModel);
        }

        /// <summary>
        /// GET: Gestor
        /// </summary>
        /// <returns>Retorna a view do dashboard do gestor</returns>
        [HttpGet, IsLogged, HavePermission(AreaNome = "DashboardGestor")]
        public ActionResult Gestor(int FiltroUsuario = 0)
        {
            var ViewModel = new ViewModelDashboardGestor();

            if (FiltroUsuario == 0 && Convert.ToString(Session["UsuarioTipo"]) == "Gestor")
            {
                FiltroUsuario = Convert.ToInt32(Session["UsuarioID"]);
            }

            int usuarioID = 0;

            //Recarrega o dropdownlist de usuários setando o valor que havia sido usado como filtro
            var Usuarios = SelectListMVC.CriaListaSelecao(GraficosDAO.Usuarios(Convert.ToInt32(Session["SetorID"])));

            foreach (var item in Usuarios)
            {
                if (Convert.ToInt32(item.Value) == Convert.ToInt32(Session["UsuarioID"]))
                {
                    item.Selected = true;
                    usuarioID = Convert.ToInt32(item.Value);
                    break;
                }
            }

            ViewBag.Usuarios = Usuarios;

            //Análise de tarefas
            ViewModel.TarefasMes = new List<int>();

            for (int i = 1; i <= 12; i++)
            {
                ViewModel.TarefasMes.Add(GraficosDAO.QuantidadeTarefasMes(i, Convert.ToInt32(Session["SetorID"]), FiltroUsuario));
            }

            //Análise de competências
            ViewModel.ListCompetencias = GraficosDAO.Listar();

            if (FiltroUsuario != 0)
            {
                foreach (var item in ViewModel.ListCompetencias)
                {
                    item.Pontos = GraficosDAO.CompetenciaPontos(item.ID, FiltroUsuario);
                }
            }
            else
            {
                foreach (var item in ViewModel.ListCompetencias)
                {
                    item.Pontos = 0;
                }
            }

            //Análise de badges
            ViewModel.ListBadgeBasicas = GraficosDAO.Badges("Basica", Convert.ToInt32(Session["SetorID"]));
            ViewModel.ListBadgeIntermediarias = GraficosDAO.Badges("Intermediaria", Convert.ToInt32(Session["SetorID"]));
            ViewModel.ListBadgeAvancadas = GraficosDAO.Badges("Avancada", Convert.ToInt32(Session["SetorID"]));

            //Busca o total de usuário da empresa, caso o usuário logado seja gestor ou funcionário busca o total de usuários do setor correspondente
            ViewModel.TotalUsuarios = GraficosDAO.TotalUsuarios(Convert.ToInt32(Session["SetorID"]));

            foreach (var item in ViewModel.ListBadgeBasicas)
            {
                ViewModel.TotalBadgesAdquiridas = GraficosDAO.BadgeAdquiridas(Convert.ToInt32(Session["SetorID"]), FiltroUsuario, item.ID);

                if (ViewModel.TotalBadgesAdquiridas != 0)
                {
                    item.Adquirida = true;
                    item.Progresso = Math.Round(((double)ViewModel.TotalBadgesAdquiridas / ViewModel.TotalUsuarios) * 100, 0);
                }
                else
                {
                    if (FiltroUsuario == 0)
                    {
                        item.Adquirida = true;
                    }
                }
            }

            foreach (var item in ViewModel.ListBadgeIntermediarias)
            {
                ViewModel.TotalBadgesAdquiridas = GraficosDAO.BadgeAdquiridas(Convert.ToInt32(Session["SetorID"]), FiltroUsuario, item.ID);

                if (ViewModel.TotalBadgesAdquiridas != 0)
                {
                    item.Adquirida = true;
                    item.Progresso = Math.Round(((double)ViewModel.TotalBadgesAdquiridas / ViewModel.TotalUsuarios) * 100, 0);
                }
                else
                {
                    if (FiltroUsuario == 0)
                    {
                        item.Adquirida = true;
                    }
                }
            }

            foreach (var item in ViewModel.ListBadgeAvancadas)
            {
                ViewModel.TotalBadgesAdquiridas = GraficosDAO.BadgeAdquiridas(Convert.ToInt32(Session["SetorID"]), FiltroUsuario, item.ID);

                if (ViewModel.TotalBadgesAdquiridas != 0)
                {
                    item.Adquirida = true;
                    item.Progresso = Math.Round(((double)ViewModel.TotalBadgesAdquiridas / ViewModel.TotalUsuarios) * 100, 0);
                }
                else
                {
                    if (FiltroUsuario == 0)
                    {
                        item.Adquirida = true;
                    }
                }
            }

            ViewBag.UsuarioID = usuarioID;

            return View(ViewModel);
        }

        /// <summary>
        /// GET: Funcionario
        /// </summary>
        /// <returns>Retorna a view do dashboard do funcionário</returns>
        [HttpGet, IsLogged, HavePermission(AreaNome = "DashboardFuncionario")]
        public ActionResult Funcionario()
        {
            var ViewModel = new ViewModelDashboardFuncionario();

            UsuariosDAO UsuariosDAO = new UsuariosDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

            ViewModel.Usuario = UsuariosDAO.Carregar(Convert.ToInt32(Session["UsuarioID"]));

            TarefasDAO TarefasDAO = new TarefasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

            //Busca as tarefa com status Fazer
            ViewModel.ListFazer = TarefasDAO.ListarTop(ConfigurationManager.AppSettings["Fazer"], 0, 3);

            CompetenciasDAO CompetenciasDAO = new CompetenciasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

            ViewModel.ListCompetencias = CompetenciasDAO.Listar();

            foreach (var item in ViewModel.ListCompetencias)
            {
                item.Pontos = CompetenciasDAO.CompetenciaPontos(item.ID);
            }

            //Cria model com as propriedades da agenda
            ViewModel.ListAgenda = new List<Agenda>();

            //Busca tarefas para montar a agenda com as mesmas, mesmo que não esteja integrado com Google as tarefas aparecem no calendário
            ViewModel.ListAgenda = TarefasDAO.ListarTarefasAgenda();

            CalendarService service = OAuthService.OAuthLogged
                               (
                                   Convert.ToString(Session["UsuarioID"]),
                                   ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString,
                                   ConfigurationManager.AppSettings["RedirectUriAgenda"] + "/Dashboard/SincronizarAgenda",
                                   "Calendar API",
                                   new[] { CalendarService.Scope.CalendarReadonly, CalendarService.Scope.Calendar }
                               );

            if (service != null)
            {
                ViewModel.ListAgenda = ViewModel.ListAgenda == null ? new List<Agenda>() : ViewModel.ListAgenda;

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
                                DataFim = Convert.ToDateTime(eventItem.End.DateTime),
                                Cor = "#3a87ad"
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
                                    ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString,
                                    ConfigurationManager.AppSettings["RedirectUriAgenda"] + "/Dashboard/SincronizarAgenda",
                                    "Calendar API",
                                    new[] { CalendarService.Scope.CalendarReadonly, CalendarService.Scope.Calendar }
                                );

            return RedirectToAction("Funcionario");
        }
    }
}