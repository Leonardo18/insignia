using Insignia.Painel.Helpers.CustomAttributes;
using System.Web.Mvc;

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
            return View();
        }

        /// <summary>
        /// POST: Agenda Visualizar
        /// </summary>
        /// <returns>Retorna a view de visualizar agenda</returns>
        [HttpPost, IsLogged]
        public ActionResult Visualizar(int i = 0)
        {
            return View();
        }
    }
}