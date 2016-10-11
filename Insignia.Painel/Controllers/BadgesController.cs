﻿using Insignia.DAO.Badges;
using Insignia.Model.Badge;
using Insignia.Painel.Helpers.CustomAttributes;
using Insignia.Painel.Helpers.Util;
using Insignia.Painel.ViewModels;
using System.Configuration;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class BadgesController : Controller
    {
        private BadgesDAO BadgesDAO = new BadgesDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);
        public string teste;

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
            //Objeto com funções de cores
            BadgesCor cor = new BadgesCor();

            var ViewModel = new ViewModelBadge();

            if (ModelState.IsValid)
            {
                BadgeModel.CorFonte = cor.HexToColor(BadgeModel.Cor);

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

        // GET: Badge Editar
        [IsLogged]
        public ActionResult Editar(int ID)
        {
            Badge BadgeModel = BadgesDAO.Load(ID);


            return View("Editar", BadgeModel);
        }

        // POST: Badge Editar           
        [IsLogged, HttpPost]
        public ActionResult Editar(Badge BadgeModel)
        {
            if (ModelState.IsValid)
            {
                if (BadgesDAO.Editar(BadgeModel))
                {
                    return RedirectToAction("Exibir", new { ID = BadgeModel.ID });
                }
            }
            return View("Editar", BadgeModel);
        }

        // GET: Badge Remover       
        [IsLogged]
        public ActionResult Remover(int ID)
        {
            //Faz Load com o ID passado
            Badge BadgeModel = BadgesDAO.Load(ID);

            return View(BadgeModel);
        }

        // POST: Badge Remover       
        [IsLogged, HttpPost]
        public ActionResult Remover(Badge BadgeModel)
        {
            if (BadgeModel != null)
            {
                if (BadgesDAO.Remover(BadgeModel.ID))
                {
                    return RedirectToAction("Adicionar");
                }
            }
            return View();
        }
    }
}