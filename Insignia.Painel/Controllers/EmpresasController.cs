using Insignia.DAO.Competencias;
using Insignia.DAO.Empresas;
using Insignia.DAO.Tarefas;
using Insignia.Painel.ViewModels;
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
        [HttpGet]
        public ActionResult Perfil(int id)
        {
            var ViewModel = new ViewModelPerfil();

            ViewModel.Empresa = EmpresaDAO.Carregar(id);

            TarefasDAO TarefasDAO = new TarefasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

            //Busca as tarefa com status finalizada e top 5
            ViewModel.ListFinalizadas = TarefasDAO.ListarTop(ConfigurationManager.AppSettings["Finalizada"], 5);

            CompetenciasDAO CompetenciasDAO = new CompetenciasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

            ViewModel.ListCompetencias = CompetenciasDAO.Listar();

            return View(ViewModel);
        }
    }
}