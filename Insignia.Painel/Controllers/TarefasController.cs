using Insignia.DAO.Tarefas;
using Insignia.Model.Tarefa;
using Insignia.Painel.Helpers.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class TarefasController : Controller
    {
        private TarefasDAO TarefasDAO = new TarefasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

        // GET: Tarefas Adicionar       
        [IsLogged]
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

        // POST: Tarefa Adicionar
        [HttpPost, IsLogged]
        public ActionResult Adicionar(Tarefa TarefaModel)
        {
            if (ModelState.IsValid)
            {
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

        // GET: Tarefa Editar
        [IsLogged]
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

        // POST: Tarefa Editar           
        [IsLogged, HttpPost]
        public ActionResult Editar(Tarefa TarefaModel)
        {
            if (ModelState.IsValid)
            {
                if (TarefasDAO.Salvar(TarefaModel))
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

        // GET: Tarefa Remover       
        [IsLogged]
        public ActionResult Remover(int ID)
        {
            //Faz Load com o ID passado
            Tarefa TarefaModel = TarefasDAO.Carregar(ID);

            return View(TarefaModel);
        }

        // POST: Tarefa Remover       
        [IsLogged, HttpPost]
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