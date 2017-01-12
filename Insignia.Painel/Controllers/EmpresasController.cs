﻿using Insignia.DAO.Competencias;
using Insignia.DAO.Empresas;
using Insignia.DAO.Tarefas;
using Insignia.Painel.Helpers.CustomAttributes;
using Insignia.Painel.ViewModels;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class EmpresasController : Controller
    {
        private EmpresasDAO EmpresaDAO = new EmpresasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

        /// <summary>
        /// GET: Empresa Perfil
        /// </summary>        
        /// <returns>Retorna a view com os dados da empresa</returns>
        [HttpGet, IsLogged]
        public ActionResult Perfil(int id)
        {
            var ViewModel = new ViewModelPerfil();

            ViewModel.Empresa = EmpresaDAO.Carregar(id);

            TarefasDAO TarefasDAO = new TarefasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

            //Busca as tarefa com status finalizada e top 5
            ViewModel.ListFinalizadas = TarefasDAO.ListarTop(ConfigurationManager.AppSettings["Finalizada"], 5);

            ViewModel.TarefasMes = new List<int>();

            for (int i = 1; i <= 12; i++)
            {
                ViewModel.TarefasMes.Add(TarefasDAO.QuantidadeTarefasMes(i));
            }

            CompetenciasDAO CompetenciasDAO = new CompetenciasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

            ViewModel.ListCompetencias = CompetenciasDAO.Listar();

            return View(ViewModel);
        }
    }
}