using Insignia.Painel.Helpers.CustomAttributes;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        //[IsLogged]
        public ActionResult Dashboard()
        {
            return View();
        }
    }
}