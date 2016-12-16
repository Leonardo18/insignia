using Insignia.DAO.Tarefas;
using Insignia.Model.Tarefa;
using Insignia.Painel.Helpers.AmazonS3;
using Insignia.Painel.Helpers.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class TarefasController : Controller
    {
        private TarefasDAO TarefasDAO = new TarefasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

        [HttpGet]
        public ActionResult Listar()
        {
            return View();
        }

        /// <summary>
        /// GET: Tarefas Adicionar 
        /// </summary>
        /// <returns>Retorna a view de adicionar tarefa</returns>
        [HttpGet, IsLogged]
        public ActionResult Adicionar()
        {
            var TarefaModel = new Tarefa();

            List<SelectListItem> TipoID = new List<SelectListItem>();

            //Busca os tipos de tarefa e retorna um dictionary contendo elas
            var TarefasTipos = TarefasDAO.Tipos();

            foreach (var item in TarefasTipos.Keys)
            {
                TipoID.Add(new SelectListItem { Text = TarefasTipos[item], Value = Convert.ToString(item) });
            }

            ViewBag.TipoID = TipoID;

            return View(TarefaModel);
        }

        /// <summary>
        /// POST: Tarefa Adicionar 
        /// </summary>
        /// <param name="TarefaModel"></param>
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
                    var Caminho = Path.Combine(Server.MapPath("~/Content/Uploads"), TarefaModel.Anexo);

                    Arquivo.SaveAs(Caminho);

                    //Verifica se existe a pasta da empresa no Bucket
                    if (!AmazonS3.ExistePasta(Convert.ToString(Session["EmpresaNome"]), ConfigurationManager.AppSettings["BucketName"]))
                    {
                        //Cria uma pasta no Bucket com o nome da empresa
                        AmazonS3.CriaPasta(Convert.ToString(Session["EmpresaNome"]), ConfigurationManager.AppSettings["BucketName"]);
                    }

                    //Faz Upload do arquivo para o S3
                    AmazonS3.EnviaArquivoS3(Caminho, ConfigurationManager.AppSettings["BucketName"], Convert.ToString(Session["EmpresaNome"]), TarefaModel.Anexo);

                    System.IO.File.Delete(Caminho);
                }

                if (TarefasDAO.Salvar(TarefaModel))
                {
                    return RedirectToAction("Editar", new { ID = TarefaModel.ID });
                }
            }

            List<SelectListItem> TipoID = new List<SelectListItem>();

            //Busca os tipos de tarefa e retorna um dictionary contendo elas
            var TarefasTipos = TarefasDAO.Tipos();

            foreach (var item in TarefasTipos.Keys)
            {
                TipoID.Add(new SelectListItem { Text = TarefasTipos[item], Value = Convert.ToString(item) });
            }

            ViewBag.TipoID = TipoID;

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

            List<SelectListItem> TipoID = new List<SelectListItem>();

            //Busca os tipos de tarefa e retorna um dictionary contendo elas
            var TarefasTipos = TarefasDAO.Tipos();

            foreach (var item in TarefasTipos.Keys)
            {
                TipoID.Add(new SelectListItem { Text = TarefasTipos[item], Value = Convert.ToString(item) });
            }

            //Retorna na list o valor marcado atualmente para o cadastro
            foreach (var item in TipoID)
            {
                if (item.Value == TarefaModel.TipoID)
                {
                    item.Selected = true;
                    break;
                }
            }
            ViewBag.TipoID = TipoID;

            return View("Editar", TarefaModel);
        }

        /// <summary>
        /// POST: Tarefa Editar
        /// </summary>
        /// <param name="TarefaModel">Model contendo os dados da Tarefa</param>
        /// <returns>Caso consiga validar os dados e atualizar a tarefa faz redirecionamento, caso contrário retorna a view novamente para ajuste de dados inválidos</returns>
        [HttpPost, IsLogged, ValidateInput(false)]
        public ActionResult Editar(Tarefa TarefaModel)
        {
            if (ModelState.IsValid)
            {
                if (TarefasDAO.Editar(TarefaModel))
                {
                    return RedirectToAction("Adicionar");
                }
            }

            List<SelectListItem> TipoID = new List<SelectListItem>();

            //Busca os tipos de tarefa e retorna um dictionary contendo elas
            var TarefasTipos = TarefasDAO.Tipos();

            foreach (var item in TarefasTipos.Keys)
            {
                TipoID.Add(new SelectListItem { Text = TarefasTipos[item], Value = Convert.ToString(item) });
            }

            ViewBag.TipoID = TipoID;

            return View("Editar", TarefaModel);
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
                if (TarefasDAO.Remover(TarefaModel.ID))
                {
                    return RedirectToAction("Adicionar");
                }
            }
            return View(TarefaModel);
        }
    }
}