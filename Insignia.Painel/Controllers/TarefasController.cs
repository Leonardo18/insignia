using Insignia.Model.Tarefa;
using Insignia.Painel.Helpers.CustomAttributes;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class TarefasController : Controller
    {
        // GET: Tarefas Adicionar       
        [IsLogged]
        public ActionResult Adicionar()
        {
            var TarefaModel = new Tarefa();

            return View(TarefaModel);
        }
    }
}