using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        //POST: Login
        [HttpPost]
        public ActionResult Login(string NomeUsuario, string Senha)
        {
            //Objeto dao da dll que contém os métodos para buscar dados e editar dados no banco.
            //Auth auth = new Auth(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

            //Tenta efeutar login com os dados passados e retorna um dictionary
            //Usuario DadosUsuario = auth.Login(NomeUsuario, Senha);

            //if (DadosUsuario != null)
            //{
            //    this.Session["SessionID"] = Session.SessionID;
            //    this.Session["UsuarioID"] = DadosUsuario.ID;
            //    this.Session["UsuarioNome"] = DadosUsuario.Nome;
            //    this.Session["UsuarioEmail"] = DadosUsuario.Email;
            //    this.Session["AssociadoID"] = DadosUsuario.AssociadoID;
            //    this.Session["Usuario"] = DadosUsuario.Login;
            //    this.Session["TipoID"] = DadosUsuario.TipoID;

            //    return RedirectToAction("../Dashboard/Dashboard");
            //}
            //else
            //{
            //    ViewBag.Error = "Usuário ou senha incorretos.";
            //}
            return View();
        }

        // POST: RecuperarSenha
        [HttpPost]
        public void RecuperarSenha(string email)
        {
            Response.Write("Email enviado para " + email);
        }

        // GET: Logout
        public ActionResult Sair()
        {
            this.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}