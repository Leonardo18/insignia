using Insignia.Painel.Helpers.CustomAttributes;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class DashboardController : Controller
    {
        /// <summary>
        /// GET: Empresa
        /// </summary>
        /// <returns>Retorna a view do dashboard da empresa</returns>
        [HttpGet, IsLogged]
        public ActionResult Empresa()
        {
            return View();
        }

        /// <summary>
        /// GET: Gestor
        /// </summary>
        /// <returns>Retorna a view do dashboard do gestor</returns>
        [HttpGet, IsLogged]
        public ActionResult Gestor()
        {
            return View();
        }

        /// <summary>
        /// GET: Funcionario
        /// </summary>
        /// <returns>Retorna a view do dashboard do funcionário</returns>
        [HttpGet, IsLogged]
        public ActionResult Funcionario()
        {
            return View();
        }
    }
}