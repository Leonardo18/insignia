﻿using Insignia.DAO.Badges;
using Insignia.DAO.Usuarios;
using Insignia.DAO.Util;
using Insignia.Model.Badge;
using Insignia.Painel.Helpers.CustomAttributes;
using Insignia.Painel.Helpers.Util;
using Insignia.Painel.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using static System.Convert;

namespace Insignia.Painel.Controllers
{
    public class BadgesController : Controller
    {
        private BadgesDAO BadgesDAO = new BadgesDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

        /// <summary>
        /// GET: Badge Adicionar
        /// </summary>
        /// <returns>Retorna a view de adicionar badge</returns>
        [HttpGet, IsLogged, HavePermission(AreaNome = "Badges")]
        public ActionResult Adicionar()
        {
            var ViewModel = new ViewModelBadge();

            ViewModel.Badge = new Badge();

            ViewModel.ListBadgeBasicas = BadgesDAO.Listar("Basica");
            ViewModel.ListBadgeIntermediarias = BadgesDAO.Listar("Intermediaria");
            ViewModel.ListBadgeAvancadas = BadgesDAO.Listar("avancada");

            //Se o usuário logado for gestor pega apenas o setor do usuário atual, se não pega todos setor por ser usuário do tipo empresa
            if (Convert.ToString(Session["UsuarioTipo"]) == "Gestor")
            {
                Dictionary<int, string> SetorGestor = new Dictionary<int, string>();

                //Faz um dicionário apenas com o setor do Gestor
                SetorGestor.Add(ToInt32(Session["SetorID"]), Database.DBBuscaInfo("Setores", "ID", Convert.ToString(Session["SetorID"]), "Nome"));

                //Cria o Select List com o setor do gestor
                ViewBag.Setores = SelectListMVC.CriaListaSelecao(SetorGestor);
            }
            else
            {
                UsuariosDAO UsuariosDAO = new UsuariosDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

                //Busca todos os setores e retorna um dictionary contendo os dados e retorna o select list
                ViewBag.Setores = SelectListMVC.CriaListaSelecao(UsuariosDAO.Setores());
            }

            return View(ViewModel);
        }

        /// <summary>
        /// POST: Badge Adicionar 
        /// </summary>
        /// <param name="BadgeModel">Objeto Model da badge contendo os dados inseridos para cadastro</param>
        /// <returns>Caso consiga validar os dados e salvar a badge faz redirecionamento, caso contrário retorna a view novamente para ajuste de dados inválidos</returns>
        [HttpPost, IsLogged, HavePermission(AreaNome = "Badges")]
        public ActionResult Adicionar(Badge BadgeModel)
        {
            //Objeto com funções de cores
            BadgesCor cor = new BadgesCor();

            var ViewModel = new ViewModelBadge();

            if (ModelState.IsValid)
            {
                BadgeModel.CorFonte = cor.HexToColor(BadgeModel.Cor);

                if (BadgesDAO.Salvar(BadgeModel))
                {
                    return RedirectToAction("../Badges/Adicionar");
                }
            }

            ViewModel.Badge = new Badge();

            ViewModel.ListBadgeBasicas = BadgesDAO.Listar("Basica");
            ViewModel.ListBadgeIntermediarias = BadgesDAO.Listar("Intermediaria");
            ViewModel.ListBadgeAvancadas = BadgesDAO.Listar("avancada");

            //Se o usuário logado for gestor pega apenas o setor do usuário atual, se não pega todos setor por ser usuário do tipo empresa
            if (Convert.ToString(Session["UsuarioTipo"]) == "Gestor")
            {
                Dictionary<int, string> SetorGestor = new Dictionary<int, string>();

                //Faz um dicionário apenas com o setor do Gestor
                SetorGestor.Add(ToInt32(Session["SetorID"]), Database.DBBuscaInfo("Setores", "ID", Convert.ToString(Session["SetorID"]), "Nome"));

                //Cria o Select List com o setor do gestor
                ViewBag.Setores = SelectListMVC.CriaListaSelecao(SetorGestor);
            }
            else
            {
                UsuariosDAO UsuariosDAO = new UsuariosDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

                //Busca todos os setores e retorna um dictionary contendo os dados
                var Setores = SelectListMVC.CriaListaSelecao(UsuariosDAO.Setores());

                foreach (var item in Setores)
                {
                    if (ToInt32(item.Value) == BadgeModel.SetorID)
                    {
                        item.Selected = true;
                        break;
                    }
                }

                ViewBag.Setores = Setores;
            }

            return View(ViewModel);
        }

        /// <summary>
        /// GET: Badge Editar
        /// </summary>
        /// <param name="ID">ID da badge que será editada</param>
        /// <returns>Retorna a view com os dados da badge a serem editados</returns>
        [HttpGet, IsLogged, HavePermission(AreaNome = "Badges")]
        public ActionResult Editar(int ID)
        {
            Badge BadgeModel = BadgesDAO.Carregar(ID);

            //Se o usuário logado for gestor pega apenas o setor do usuário atual, se não pega todos setor por ser usuário do tipo empresa
            if (Convert.ToString(Session["UsuarioTipo"]) == "Gestor")
            {
                Dictionary<int, string> SetorGestor = new Dictionary<int, string>();

                //Faz um dicionário apenas com o setor do Gestor
                SetorGestor.Add(ToInt32(Session["SetorID"]), Database.DBBuscaInfo("Setores", "ID", Convert.ToString(Session["SetorID"]), "Nome"));

                //Cria o Select List com o setor do gestor
                ViewBag.Setores = SelectListMVC.CriaListaSelecao(SetorGestor);
            }
            else
            {
                UsuariosDAO UsuariosDAO = new UsuariosDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

                //Busca os setores e retorna um dictionary contendo os dados
                var Setores = SelectListMVC.CriaListaSelecao(UsuariosDAO.Setores());

                foreach (var item in Setores)
                {
                    if (ToInt32(item.Value) == BadgeModel.SetorID)
                    {
                        item.Selected = true;
                        break;
                    }
                }

                ViewBag.Setores = Setores;
            }

            return View("Editar", BadgeModel);
        }

        /// <summary>
        /// POST: Badge Editar 
        /// </summary>
        /// <param name="BadgeModel">Objeto Model da badge contendo os dados atualiados</param>
        /// <returns>Caso consiga validar os dados e atualizar a badge faz redirecionamento, caso contrário retorna a view novamente para ajuste de dados inválidos</returns>
        [HttpPost, IsLogged, HavePermission(AreaNome = "Badges")]
        public ActionResult Editar(Badge BadgeModel)
        {
            //Objeto com funções de cores
            BadgesCor cor = new BadgesCor();

            if (ModelState.IsValid)
            {
                BadgeModel.CorFonte = cor.HexToColor(BadgeModel.Cor);

                if (BadgesDAO.Editar(BadgeModel))
                {
                    return RedirectToAction("Adicionar");
                }
            }

            return View("Editar", BadgeModel);
        }

        /// <summary>
        /// GET: Badge Remover 
        /// </summary>
        /// <param name="ID">ID da bagde a ser removida</param>
        /// <returns>Retorna a view com dados da badge que será removida</returns>
        [HttpGet, IsLogged, HavePermission(AreaNome = "Badges")]
        public ActionResult Remover(int ID)
        {
            //Faz Load com o ID passado
            Badge BadgeModel = BadgesDAO.Carregar(ID);

            return View(BadgeModel);
        }

        /// <summary>
        /// POST: Badge Remover
        /// </summary>
        /// <param name="BadgeModel">Model da badge contendo dados dela</param>
        /// <returns>Caso consiga remover a badge do sistema faz redirecionamento, caso contrário retorna a view com mensagem</returns>
        [HttpPost, IsLogged, HavePermission(AreaNome = "Badges")]
        public ActionResult Remover(Badge BadgeModel)
        {
            //Faz Load com o ID passado
            BadgeModel = BadgesDAO.Carregar(BadgeModel.ID);

            if (BadgeModel != null)
            {
                if (!BadgesDAO.PodeRemover(BadgeModel.ID))
                {
                    if (BadgesDAO.Remover(BadgeModel.ID))
                    {
                        return RedirectToAction("Adicionar");
                    }
                }
                else
                {
                    ViewBag.Error = "Não é possível remover a badge " + BadgeModel.Titulo + ", pois existem tarefas cadastras para a mesma.";
                }
            }

            return View(BadgeModel);
        }

        /// <summary>
        /// GET: Badges Adquiridas
        /// </summary>        
        /// <returns>Retorna a view com dados das badges adquiridas</returns>
        [HttpGet, IsLogged]
        public ActionResult Adquiridas()
        {
            var ViewModel = new ViewModelBadge();

            ViewModel.ListBadgeBasicas = BadgesDAO.Listar("Basica");
            ViewModel.ListBadgeIntermediarias = BadgesDAO.Listar("Intermediaria");
            ViewModel.ListBadgeAvancadas = BadgesDAO.Listar("Avancada");

            foreach (var item in ViewModel.ListBadgeBasicas)
            {
                item.Adquirida = BadgesDAO.VerificaBadgeAdquirida(item.ID, ToInt32(Session["UsuarioID"]));
            }

            foreach (var item in ViewModel.ListBadgeIntermediarias)
            {
                item.Adquirida = BadgesDAO.VerificaBadgeAdquirida(item.ID, ToInt32(Session["UsuarioID"]));
            }

            foreach (var item in ViewModel.ListBadgeAvancadas)
            {
                item.Adquirida = BadgesDAO.VerificaBadgeAdquirida(item.ID, ToInt32(Session["UsuarioID"]));
            }

            return View(ViewModel);
        }
    }
}