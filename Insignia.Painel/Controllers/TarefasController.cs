using Insignia.DAO.Tarefas;
using Insignia.Model.Tarefa;
using Insignia.Painel.Helpers.AmazonS3;
using Insignia.Painel.Helpers.CustomAttributes;
using Insignia.Painel.ViewModels;
using System;
using System.Configuration;
using System.IO;
using System.Web;
using Insignia.Painel.Helpers.Util;
using System.Web.Mvc;
using Insignia.DAO.Util;
using System.Collections.Generic;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Insignia.DAO.Autenticacao.Google;

namespace Insignia.Painel.Controllers
{
    public class TarefasController : Controller
    {
        private TarefasDAO TarefasDAO = new TarefasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

        /// <summary>
        /// GET: Tarefas Listar
        /// </summary>
        /// <returns>Retorna a view de listar tarefas com os dados</returns>
        [HttpGet, IsLogged]
        public ActionResult Listar()
        {
            var ViewModel = new ViewModelTarefa();

            //Busca as tarefa com status a fazer
            ViewModel.ListFazer = TarefasDAO.Listar(ConfigurationManager.AppSettings["Fazer"]);

            //Busca as tarefa com status em andamento
            ViewModel.ListAndamento = TarefasDAO.Listar(ConfigurationManager.AppSettings["Andamento"]);

            //Busca as tarefa com status finalizada
            ViewModel.ListFinalizadas = TarefasDAO.Listar(ConfigurationManager.AppSettings["Finalizada"]);

            //Busca as tarefa em que o usuário logado é participante
            var Participantes = TarefasDAO.ListarParticipante(0, 5);

            //Busca nome do usuário que criou a tarefa
            foreach (var item in Participantes)
            {
                item.UsuarioNome = Database.DBBuscaInfo("Usuarios", "ID", Convert.ToString(item.UsuarioID), "Nome");
            }

            ViewModel.ListParticipante = Participantes;

            return View(ViewModel);
        }

        /// <summary>
        /// GET: Tarefas Adicionar
        /// </summary>
        /// <returns>Retorna a view de adicionar tarefa</returns>
        [HttpGet, IsLogged]
        public ActionResult Adicionar()
        {
            var TarefaModel = new Tarefa();

            //Busca os tipos de tarefa e retorna um dictionary contendo os registros para montar o select list
            ViewBag.TipoID = SelectListMVC.CriaListaSelecao(TarefasDAO.Tipos());

            //Busca os usuários retorna um dictionary contendo os registros para montar o select list de participantes
            ViewBag.Participantes = SelectListMVC.CriaListaSelecao(TarefasDAO.Participantes());

            return View(TarefaModel);
        }

        /// <summary>
        /// POST: Tarefa Adicionar 
        /// </summary>
        /// <param name="TarefaModel">Objeto Model da tarefa contendo os dados inseridos para cadastro</param>
        /// <returns>Caso consiga validar e salvar a tarefa faz redirecionamento, se não retorna a view com mensagem</returns>
        [HttpPost, IsLogged, ValidateInput(false)]
        public ActionResult Adicionar(Tarefa TarefaModel, HttpPostedFileBase Arquivo)
        {
            if (ModelState.IsValid)
            {
                // Verifica se existe um arquivo escolhido
                if (Arquivo != null && Arquivo.ContentLength > 0)
                {
                    AmazonUpload AmazonS3 = new AmazonUpload();

                    // Pega o nome do arquivo
                    TarefaModel.Anexo = Path.GetFileName(Arquivo.FileName);

                    // Grava o arquivo em uma pasta local
                    var Caminho = Path.Combine(Server.MapPath("~/Content/uploads"), TarefaModel.Anexo);
                    Arquivo.SaveAs(Caminho);

                    //Verifica se existe a pasta da empresa no Bucket
                    if (!AmazonS3.ExistePasta(Convert.ToString(Session["EmpresaNome"]), "Arquivos", ConfigurationManager.AppSettings["BucketName"]))
                    {
                        //Cria uma pasta no Bucket com o nome da empresa
                        AmazonS3.CriaPasta(Convert.ToString(Session["EmpresaNome"]), "Arquivos", ConfigurationManager.AppSettings["BucketName"]);
                    }

                    //Faz Upload do arquivo para o S3
                    AmazonS3.EnviaArquivoS3(Caminho, ConfigurationManager.AppSettings["BucketName"], Convert.ToString(Session["EmpresaNome"]), "Arquivos", TarefaModel.Anexo);

                    //Deleta o arquivo salvo local
                    System.IO.File.Delete(Caminho);
                }

                TarefaModel.Status = ConfigurationManager.AppSettings["Fazer"];

                CalendarService service = null;

                //Caso exista login do usuário na tabela de goole, busca serviço
                if (!string.IsNullOrEmpty(Database.DBBuscaInfo("UsuariosGoogle", "UsuarioID", Convert.ToString(Session["UsuarioID"]), "Usuario")))
                {
                    service = OAuthService.OAuthLogged
                                         (
                                             Convert.ToString(Session["UsuarioID"]),
                                             ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString, "https://www.portalinsignia.com.br/Agenda/SincronizarAgenda",
                                             "Calendar API",
                                             new[] { CalendarService.Scope.CalendarReadonly, CalendarService.Scope.Calendar }
                                         );
                }

                if (service != null)
                {
                    do
                    {
                        TarefaModel.EventoID = Convert.ToInt64(Database.GeraCodigo(10));
                    } while (TarefasDAO.VerificaEventoID(TarefaModel.EventoID));
                }

                if (TarefasDAO.Salvar(TarefaModel))
                {
                    //Se possui integração com google calendar, adiciona a tarefa no google
                    if (service != null)
                    {
                        //Inserção de evento da tarefa recém cadastrada
                        Event NovoEvento = new Event();

                        NovoEvento.Id = Convert.ToString(TarefaModel.EventoID);
                        NovoEvento.Summary = TarefaModel.Titulo;
                        NovoEvento.Description = TarefaModel.Descricao;
                        NovoEvento.Start = new EventDateTime();
                        NovoEvento.Start.DateTime = TarefaModel.Termino;
                        NovoEvento.End = new EventDateTime();
                        NovoEvento.End.DateTime = TarefaModel.Termino;
                        var eventResult = service.Events.Insert(NovoEvento, "primary").Execute();
                    }

                    return RedirectToAction("Listar");
                }
            }

            //Busca os tipos de tarefa e retorna um dictionary contendo os registros e monta o select list
            var TarefasTipos = SelectListMVC.CriaListaSelecao(TarefasDAO.Tipos());

            foreach (var item in TarefasTipos)
            {
                if (item.Value == TarefaModel.TipoID)
                {
                    item.Selected = true;
                    break;
                }
            }

            ViewBag.TipoID = TarefasTipos;
            ViewBag.Participantes = SelectListMVC.CriaListaSelecao(TarefasDAO.Participantes());

            return View(TarefaModel);
        }

        /// <summary>
        /// GET: Tarefa Editar 
        /// </summary>
        /// <param name="ID">ID da tarefa a ser editada</param>
        /// <returns>Retorna a view com os dados da tarefa a serem editados</returns>
        [HttpGet, IsLogged]
        public ActionResult Editar(int ID)
        {
            Tarefa TarefaModel = TarefasDAO.Carregar(ID);

            //Busca os tipos de tarefa e retorna um dictionary contendo elas
            var TarefasTipos = SelectListMVC.CriaListaSelecao(TarefasDAO.Tipos());

            //Retorna na list o valor marcado atualmente para a tarefa
            foreach (var item in TarefasTipos)
            {
                if (item.Value == TarefaModel.TipoID)
                {
                    item.Selected = true;
                    break;
                }
            }

            ViewBag.TipoID = TarefasTipos;
            ViewBag.Participantes = SelectListMVC.CriaListaSelecao(TarefasDAO.Participantes());

            return View("Editar", TarefaModel);
        }

        /// <summary>
        /// POST: Tarefa Editar
        /// </summary>
        /// <param name="TarefaModel">Model contendo os dados da Tarefa</param>
        /// <returns>Caso consiga validar os dados e atualizar a tarefa faz redirecionamento, caso contrário retorna a view novamente para ajuste de dados inválidos</returns>
        [HttpPost, IsLogged, ValidateInput(false)]
        public ActionResult Editar(Tarefa TarefaModel, HttpPostedFileBase Arquivo)
        {
            if (ModelState.IsValid)
            {
                // Verifica se existe um arquivo escolhido
                if (Arquivo != null && Arquivo.ContentLength > 0)
                {
                    AmazonUpload AmazonS3 = new AmazonUpload();

                    //Verifica se possui arquivo antigo para substituição na amazon
                    string ArquivoAntigo = TarefasDAO.BuscaArquivo(TarefaModel.ID);

                    // Pega o nome do arquivo
                    TarefaModel.Anexo = Path.GetFileName(Arquivo.FileName);

                    // Grava o arquivo em uma pasta local
                    var Caminho = Path.Combine(Server.MapPath("~/Content/uploads"), TarefaModel.Anexo);
                    Arquivo.SaveAs(Caminho);

                    //Verifica se existe a pasta da empresa no Bucket
                    if (!AmazonS3.ExistePasta(Convert.ToString(Session["EmpresaNome"]), "Arquivos", ConfigurationManager.AppSettings["BucketName"]))
                    {
                        //Cria uma pasta no Bucket com o nome da empresa
                        AmazonS3.CriaPasta(Convert.ToString(Session["EmpresaNome"]), "Arquivos", ConfigurationManager.AppSettings["BucketName"]);
                    }

                    //Apaga arquivo antigo para fazer upload de um novo
                    AmazonS3.ApagaArquivo(ConfigurationManager.AppSettings["BucketName"], Convert.ToString(Session["EmpresaNome"]), "Arquivos", ArquivoAntigo);

                    //Faz Upload do arquivo para o S3
                    AmazonS3.EnviaArquivoS3(Caminho, ConfigurationManager.AppSettings["BucketName"], Convert.ToString(Session["EmpresaNome"]), "Arquivos", TarefaModel.Anexo);

                    //Deleta o arquivo salvo Local
                    System.IO.File.Delete(Caminho);
                }
                else
                {
                    //Se não tem arquivo nome, mantém o antigo
                    TarefaModel.Anexo = TarefasDAO.BuscaArquivo(TarefaModel.ID);
                }

                if (TarefasDAO.Editar(TarefaModel))
                {
                    CalendarService service = null;

                    //Caso exista login do usuário na tabela de goole, busca serviço
                    if (!string.IsNullOrEmpty(Database.DBBuscaInfo("UsuariosGoogle", "UsuarioID", Convert.ToString(Session["UsuarioID"]), "Usuario")))
                    {
                        service = OAuthService.OAuthLogged
                                             (
                                                 Convert.ToString(Session["UsuarioID"]),
                                                 ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString, "https://www.portalinsignia.com.br/Agenda/SincronizarAgenda",
                                                 "Calendar API",
                                                 new[] { CalendarService.Scope.CalendarReadonly, CalendarService.Scope.Calendar }
                                             );
                    }

                    //Se possui integração com google calendar, adiciona a tarefa no google
                    if (service != null && Database.DBBuscaInfo("Tarefas", "ID", Convert.ToString(TarefaModel.ID), "EventoID") != "0")
                    {
                        //Atualiza o evento da tarefa no google calendar
                        Event NovoEvento = new Event();

                        NovoEvento.Summary = TarefaModel.Titulo;
                        NovoEvento.Description = TarefaModel.Descricao;
                        NovoEvento.Start = new EventDateTime();
                        NovoEvento.Start.DateTime = TarefaModel.Termino;
                        NovoEvento.End = new EventDateTime();
                        NovoEvento.End.DateTime = TarefaModel.Termino;
                        var eventResult = service.Events.Update(NovoEvento, "primary", Database.DBBuscaInfo("Tarefas", "ID", Convert.ToString(TarefaModel.ID), "EventoID")).Execute();
                    }
                    else if (service != null && Database.DBBuscaInfo("Tarefas", "ID", Convert.ToString(TarefaModel.ID), "EventoID") == "0")
                    {
                        do
                        {
                            TarefaModel.EventoID = Convert.ToInt64(Database.GeraCodigo(10));
                        } while (TarefasDAO.VerificaEventoID(TarefaModel.EventoID));

                        TarefasDAO.AtualizaEventoID(TarefaModel.ID, TarefaModel.EventoID);

                        //Inserção de evento da tarefa editada e que não está no google calendar
                        Event NovoEvento = new Event();

                        NovoEvento.Id = Convert.ToString(TarefaModel.EventoID);
                        NovoEvento.Summary = TarefaModel.Titulo;
                        NovoEvento.Description = TarefaModel.Descricao;
                        NovoEvento.Start = new EventDateTime();
                        NovoEvento.Start.DateTime = TarefaModel.Termino;
                        NovoEvento.End = new EventDateTime();
                        NovoEvento.End.DateTime = TarefaModel.Termino;
                        var eventResult = service.Events.Insert(NovoEvento, "primary").Execute();
                    }

                    return RedirectToAction("Editar", new { ID = TarefaModel.ID });
                }
            }

            //Busca os tipos de tarefa e retorna um dictionary contendo elas
            var TarefasTipos = SelectListMVC.CriaListaSelecao(TarefasDAO.Tipos());

            foreach (var item in TarefasTipos)
            {
                if (item.Value == TarefaModel.TipoID)
                {
                    item.Selected = true;
                    break;
                }
            }

            ViewBag.TipoID = TarefasTipos;

            return View("Editar", TarefaModel);
        }

        /// <summary>
        /// Chamado por ajax, atualiza o status de uma tarefa e verifica se conquista badge
        /// </summary>
        /// <param name="ID">ID da Tarefa no qual o status será atualizado</param>
        /// <param name="ID">Status que a tarefa irá ficar</param>
        /// <returns>Busca a tarefa e atualiza seu status</returns>                
        public string AtualizaStatus(int ID, string Status)
        {
            string resp = string.Empty;

            if (TarefasDAO.AtualizaStatus(ID, Status))
            {
                if (Status == ConfigurationManager.AppSettings["Finalizada"])
                {
                    Tarefa TarefaModel = TarefasDAO.Carregar(ID);

                    if (!string.IsNullOrEmpty(TarefaModel.TipoID))
                    {
                        resp = Convert.ToString(TarefasDAO.VerificaBadge(TarefaModel.TipoID, TarefaModel.UsuarioID));
                    }
                }
            }

            return resp;
        }

        /// <summary>
        /// GET: Tarefa Remover
        /// </summary>
        /// <param name="ID">ID da tarefa a ser removida</param>
        /// <returns>Retorna a view com dados da tarefa que será removida</returns>
        [HttpGet, IsLogged]
        public ActionResult Remover(int ID)
        {
            //Faz Load com o ID passado
            Tarefa TarefaModel = TarefasDAO.Carregar(ID);

            return View(TarefaModel);
        }

        /// <summary>
        /// POST: Tarefa Remover
        /// </summary>
        /// <param name="TarefaModel">Model contendo os dados da tarefa</param>
        /// <returns>Caso consiga remover a tarefa do sistema faz redirecionamento, caso contrário retorna a view com mensagem</returns>
        [HttpPost, IsLogged]
        public ActionResult Remover(Tarefa TarefaModel)
        {
            //Faz Load com o ID passado
            TarefaModel = TarefasDAO.Carregar(TarefaModel.ID);

            if (TarefaModel != null)
            {
                AmazonUpload AmazonS3 = new AmazonUpload();

                //Apaga arquivo antes de apagar a tarefa
                if (AmazonS3.ApagaArquivo(ConfigurationManager.AppSettings["BucketName"], Convert.ToString(Session["EmpresaNome"]), "Arquivos", TarefaModel.Anexo))
                {
                    if (TarefasDAO.Remover(TarefaModel.ID))
                    {
                        CalendarService service = null;

                        //Caso exista login do usuário na tabela de goole, busca serviço
                        if (!string.IsNullOrEmpty(Database.DBBuscaInfo("UsuariosGoogle", "UsuarioID", Convert.ToString(Session["UsuarioID"]), "Usuario")))
                        {
                            service = OAuthService.OAuthLogged
                                                 (
                                                     Convert.ToString(Session["UsuarioID"]),
                                                     ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString, "https://www.portalinsignia.com.br/Agenda/SincronizarAgenda",
                                                     "Calendar API",
                                                     new[] { CalendarService.Scope.CalendarReadonly, CalendarService.Scope.Calendar }
                                                 );
                        }

                        //Se possui integração com google calendar, adiciona a tarefa no google
                        if (service != null && TarefaModel.EventoID != 0)
                        {
                            var eventResult = service.Events.Delete("primary", Convert.ToString(TarefaModel.EventoID)).Execute();
                        }

                        return RedirectToAction("Listar");
                    }
                    else
                    {
                        ViewBag.Error = "Ocorreu um erro ao tentar excluir o resgistro, favor entrar em contato com o administrador do sistema";
                    }
                }
                else
                {
                    ViewBag.Error = "Ocorreu um erro ao tentar excluir o resgistro, favor entrar em contato com o administrador do sistema";
                }
            }

            return View(TarefaModel);
        }

        /// <summary>
        /// Ativado por um ajax que carrega uma listagem de tarefas a partir dos index
        /// </summary>
        /// <param name="tarefaTipo">Status da Tarefa</param>
        /// <param name="index">Index no qual até quanto será buscado</param>
        /// <returns>Retorna uma list contendo as tarefas do resultado da consulta</returns>
        public ActionResult CarregarMais(string tarefaStatus, int index)
        {
            List<Tarefa> list = tarefaStatus == "Participante" ? TarefasDAO.ListarParticipante(index, index + 5) : TarefasDAO.ListarTop(tarefaStatus, index, index + 5);

            return Json(new { list = list }, JsonRequestBehavior.AllowGet);
        }
    }
}