using Insignia.DAO.Autenticacao;
using Insignia.Model.Empresa;
using Insignia.Model.Usuario;
using System.Configuration;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View(new Insignia.Model.Empresa.Empresa());
        }

        //POST: Login
        [HttpPost]
        public ActionResult Login(string Email, string Senha)
        {
            //Objeto dao da dll que contém os métodos para buscar dados e editar dados no banco.
            Auth auth = new Auth(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

            //Tenta efeutar login com os dados passados e retorna um dictionary
            Usuario DadosUsuario = auth.LoginUsuario(Email, Senha);

            if (DadosUsuario != null)
            {
                //this.Session["SessionID"] = Session.SessionID;
                //this.Session["UsuarioID"] = DadosUsuario.ID;
                //this.Session["UsuarioNome"] = DadosUsuario.Nome;
                //this.Session["UsuarioEmail"] = DadosUsuario.Email;
                //this.Session["Usuario"] = DadosUsuario.Login;
                //this.Session["TipoID"] = DadosUsuario.TipoID;

                return RedirectToAction("../Dashboard/Dashboard");
            }
            else
            {
                ViewBag.Error = "E-mail ou senha incorretos.";
            }
            return View();
        }


        // POST: RecuperarSenha
        [HttpPost]
        public ActionResult NovaConta(Empresa EmpresaDados)
        {
            if (ModelState.IsValid)
            {

            }

            return View("Login", EmpresaDados);
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