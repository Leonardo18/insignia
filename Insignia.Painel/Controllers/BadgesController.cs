using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class BadgesController : Controller
    {
        // GET: Badges
        public ActionResult Adicionar()
        {
            return View();
        }
    }
}