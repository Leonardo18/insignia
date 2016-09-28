﻿using Insignia.DAO.Badges;
using Insignia.Model.Badge;
using Insignia.Painel.Helpers.CustomAttributes;
using Insignia.Painel.ViewModels;
using System.Configuration;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class BadgesController : Controller
    {
        private BadgesDAO BadgesDAO = new BadgesDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

        // GET: Badge Adicionar
        [IsLogged]
        public ActionResult Adicionar()
        {
            var ViewModel = new ViewModelBadge();

            ViewModel.Badge = new Badge();
            ViewModel.ListBadge = BadgesDAO.Listar();

            return View(ViewModel);
        }

        // POST: Badge Adicionar
        [HttpPost, IsLogged]
        public ActionResult Adicionar(Badge BadgeModel)
        {
            var ViewModel = new ViewModelBadge();

            if (ModelState.IsValid)
            {
                if (BadgesDAO.Save(BadgeModel))
                {
                    return RedirectToAction("../Badges/Adicionar");
                }
            }

            ViewModel.Badge = new Badge();
            ViewModel.ListBadge = BadgesDAO.Listar();

            return View(ViewModel);
        }

        // GET: Badge Exibir       
        [IsLogged]
        public ActionResult Exibir(int ID)
        {
            //Faz Load com o ID passado
            Badge BadgeModel = BadgesDAO.Load(ID);

            return View(BadgeModel);
        }
    }
}