using Insignia.DAO.Badges;
using Insignia.Model.Badge;
using Insignia.Painel.Helpers.CustomAttributes;
using Insignia.Painel.ViewModels;
using System.Configuration;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class BadgesController : Controller
    {
        private BadgesDAO dao = new BadgesDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

        // GET: Adicionar
        [IsLogged]
        public ActionResult Adicionar()
        {
            var ViewModel = new ViewModelBadge();

            ViewModel.Badge = new Badge();
            ViewModel.ListBadge = dao.Listar();

            return View(ViewModel);
        }

        // POST: Adicionar
        [HttpPost, IsLogged]
        public ActionResult Adicionar(Badge BadgeModel)
        {
            var ViewModel = new ViewModelBadge();

            if (ModelState.IsValid)
            {
                if (dao.Save(BadgeModel))
                {
                    return RedirectToAction("../Badges/Adicionar");
                }
            }

            ViewModel.Badge = new Badge();
            ViewModel.ListBadge = dao.Listar();

            return View(ViewModel);
        }
    }
}