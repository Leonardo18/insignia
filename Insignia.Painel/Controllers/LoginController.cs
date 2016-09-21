using Insignia.DAO.Empresas;
using Insignia.Model.Empresa;
using System.Configuration;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class LoginController : Controller
    {
        private EmpresasDAO dao = new EmpresasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

        // GET: Login
        public ActionResult Login()
        {
            return View(new Empresa());
        }

        //POST: Login
        [HttpPost]
        public ActionResult Login(string Email, string Senha)
        {
            //Objeto dao da dll que contém os métodos para buscar dados e editar dados no banco.
            //Auth auth = new Auth(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

            //Tenta efeutar login com os dados passados e retorna um dictionary
            //Usuario DadosUsuario = auth.LoginUsuario(Email, Senha);

            //if (DadosUsuario != null)
            //{
            //    this.Session["SessionID"] = Session.SessionID;
            //    this.Session["UsuarioID"] = DadosUsuario.ID;
            //    this.Session["UsuarioNome"] = DadosUsuario.Nome;
            //    this.Session["UsuarioEmail"] = DadosUsuario.Email;
            //    this.Session["Usuario"] = DadosUsuario.Login;
            //    this.Session["TipoID"] = DadosUsuario.TipoID;

            //    return RedirectToAction("../Dashboard/Dashboard");
            //}
            //else
            //{
            //    ViewBag.Error = "E-mail ou senha incorretos.";
            //}
            return View();
        }


        // POST: NovaConta
        [HttpPost]
        public ActionResult NovaConta(Empresa EmpresaModel)
        {
            if (ModelState.IsValid)
            {
                if (dao.Save(EmpresaModel))
                {
                    this.Session["SessionID"] = Session.SessionID;
                    this.Session["EmpresaID"] = EmpresaModel.ID;
                    this.Session["EmpresaRazao"] = EmpresaModel.RazaoSocial;
                    this.Session["EmpresaEmail"] = EmpresaModel.Email;

                    return RedirectToAction("../Dashboard/Dashboard");
                }
            }
            return View("Login", EmpresaModel);
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