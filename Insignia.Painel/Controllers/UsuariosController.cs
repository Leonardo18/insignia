using Insignia.DAO.Usuarios;
using Insignia.DAO.Util;
using Insignia.Model.Usuario;
using Insignia.Painel.Helpers.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class UsuariosController : Controller
    {
        private UsuariosDAO UsuariosDAO = new UsuariosDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

        /// <summary>
        /// GET: Usuário Listar
        /// </summary>
        /// <returns>Retorna a view de listar usuários com os dados</returns>
        [HttpGet, IsLogged]
        public ActionResult Listar()
        {
            var UsuarioModel = UsuariosDAO.Listar();

            foreach (var item in UsuarioModel)
            {
                item.Setor = Database.DBBuscaInfo("Setores", "ID", Convert.ToString(item.SetorID), "Nome");
            }

            return View(UsuarioModel);
        }

        /// <summary>
        /// GET: Usuário Adicionar
        /// </summary>
        /// <returns>Retorna a view de adicionar usuários</returns>
        [HttpGet, IsLogged]
        public ActionResult Adicionar()
        {
            var UsuarioModel = new Usuario();

            List<SelectListItem> Setores = new List<SelectListItem>();

            //Busca os tipos de tarefa e retorna um dictionary contendo os dados
            var SetoresNome = UsuariosDAO.Setores();

            foreach (var item in SetoresNome.Keys)
            {
                Setores.Add(new SelectListItem { Text = SetoresNome[item], Value = Convert.ToString(item) });
            }

            ViewBag.Setores = Setores;

            return View(UsuarioModel);
        }

        /// <summary>
        /// POST: Usuario Adicionar 
        /// </summary>
        /// <param name="UsuarioModel">Objeto Model do usuário contendo os dados inseridos para cadastro</param>
        /// <returns>Caso consiga validar e salvar faz redirecionamento, se não retorna a view com mensagem</returns>
        [HttpPost, IsLogged]
        public ActionResult Adicionar(Usuario UsuarioModel)
        {
            if (ModelState.IsValid)
            {
                if (UsuariosDAO.VerificaUsuario(UsuarioModel.Email) && string.IsNullOrEmpty(Database.DBBuscaInfo("Empresas", "Email", UsuarioModel.Email, "ID")))
                {
                    if (UsuariosDAO.Salvar(UsuarioModel))
                    {
                        return RedirectToAction("Editar", new { ID = UsuarioModel.ID });
                    }
                }
                else
                {
                    ViewBag.Error = "O Usuário " + UsuarioModel.Nome + " já possui um cadastro.";
                }
            }

            List<SelectListItem> Setores = new List<SelectListItem>();

            //Busca os  e retorna um dictionary contendo os dados
            var UsuariosTipos = UsuariosDAO.Setores();

            foreach (var item in UsuariosTipos.Keys)
            {
                Setores.Add(new SelectListItem { Text = UsuariosTipos[item], Value = Convert.ToString(item) });
            }

            ViewBag.Setores = Setores;

            return View(UsuarioModel);
        }

        /// <summary>
        /// GET: Usuário Editar 
        /// </summary>
        /// <param name="ID">ID do usuário a ser editado</param>
        /// <returns>Retorna a view com os dados do usuário a serem editados</returns>
        [HttpGet, IsLogged]
        public ActionResult Editar(int ID)
        {
            Usuario UsuarioModel = UsuariosDAO.Carregar(ID);

            List<SelectListItem> Setores = new List<SelectListItem>();

            //Busca os tipos de tarefa e retorna um dictionary contendo elas
            var UsuariosTipos = UsuariosDAO.Setores();

            foreach (var item in UsuariosTipos.Keys)
            {
                Setores.Add(new SelectListItem { Text = UsuariosTipos[item], Value = Convert.ToString(item) });
            }

            //Retorna na list o valor marcado atualmente para o cadastro
            foreach (var item in Setores)
            {
                if (item.Value == Convert.ToString(UsuarioModel.SetorID))
                {
                    item.Selected = true;
                    break;
                }
            }

            ViewBag.Setores = Setores;

            return View("Editar", UsuarioModel);
        }

        /// <summary>
        /// POST: Usuário Editar
        /// </summary>
        /// <param name="UsuarioModel">Model contendo os dados do Usuario</param>
        /// <returns>Caso consiga validar os dados e atualizar o usuário faz redirecionamento, caso contrário retorna a view novamente para ajuste de dados inválidos</returns>
        [HttpPost, IsLogged]
        public ActionResult Editar(Usuario UsuarioModel)
        {
            if (ModelState.IsValid)
            {
                if (UsuariosDAO.VerificaUsuario(UsuarioModel.Email) && string.IsNullOrEmpty(Database.DBBuscaInfo("Empresas", "Email", UsuarioModel.Email, "ID")))
                {
                    if (UsuariosDAO.Editar(UsuarioModel))
                    {
                        return RedirectToAction("Editar", new { ID = UsuarioModel.ID });
                    }
                }
            }

            List<SelectListItem> Setores = new List<SelectListItem>();

            //Busca os setores e retorna um dictionary contendo elas
            var UsuariosTipos = UsuariosDAO.Setores();

            foreach (var item in UsuariosTipos.Keys)
            {
                Setores.Add(new SelectListItem { Text = UsuariosTipos[item], Value = Convert.ToString(item) });
            }

            ViewBag.Setores = Setores;

            return View("Editar", UsuarioModel);
        }

        /// <summary>
        /// GET: Usuário Remover
        /// </summary>
        /// <param name="ID">ID do Usuário a ser removido</param>
        /// <returns>Retorna a view com dados do usuário que será removido</returns>
        [HttpGet, IsLogged]
        public ActionResult Remover(int ID)
        {
            //Faz Load com o ID passado
            Usuario UsuarioModel = UsuariosDAO.Carregar(ID);

            return View(UsuarioModel);
        }

        /// <summary>
        /// POST: Usuário Remover
        /// </summary>
        /// <param name="UsuarioModel">Model contendo os dados do Usuário</param>
        /// <returns>Caso consiga remover o Usuário do sistema faz redirecionamento, caso contrário retorna a view com mensagem</returns>
        [HttpPost, IsLogged]
        public ActionResult Remover(Usuario UsuarioModel)
        {
            //Faz Load com o ID passado
            UsuarioModel = UsuariosDAO.Carregar(UsuarioModel.ID);

            if (UsuarioModel != null)
            {
                if (UsuariosDAO.Remover(UsuarioModel.ID))
                {
                    return RedirectToAction("Listar");
                }
            }

            return View(UsuarioModel);
        }
    }
}