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

        // POST: Badge Adicionar
        [HttpPost, IsLogged]
        public ActionResult Adicionar(Tarefa TarefaModel)
        {
            if (ModelState.IsValid)
            {
                if (TarefasDAO.Salvar(TarefaModel))
                {
                    return RedirectToAction("../Tarefas/Exibir");
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
    }
}