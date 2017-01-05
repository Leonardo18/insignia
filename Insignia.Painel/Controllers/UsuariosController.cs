using Insignia.DAO.Usuarios;
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

            //Busca os tipos de tarefa e retorna um dictionary contendo elas
            var UsuariosTipos = UsuariosDAO.Setores();

            foreach (var item in UsuariosTipos.Keys)
            {
                Setores.Add(new SelectListItem { Text = UsuariosTipos[item], Value = Convert.ToString(item) });
            }

            ViewBag.Setores = Setores;

            return View(UsuarioModel);
        }
    }
}