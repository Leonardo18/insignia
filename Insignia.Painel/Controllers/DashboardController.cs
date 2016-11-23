using Insignia.Painel.Helpers.CustomAttributes;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class DashboardController : Controller
    {
        /// <summary>
        /// GET: Dashboard 
        /// </summary>
        /// <returns>Retorna a view do dashboard</returns>
        [HttpGet, IsLogged]
        public ActionResult Dashboard()
        {
            return View();
        }
    }
}