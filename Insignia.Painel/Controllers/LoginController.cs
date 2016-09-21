using Insignia.DAO.Autenticacao;
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
            Auth auth = new Auth(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

            //Tenta efeutar login com os dados passados e retorna um dictionary
            Empresa EmpresaDados = auth.LoginEmpresa(Email, Senha);

            if (EmpresaDados != null)
            {
                this.Session["SessionID"] = Session.SessionID;
                this.Session["UsuarioID"] = EmpresaDados.ID;
                this.Session["UsuarioNome"] = EmpresaDados.RazaoSocial;
                this.Session["UsuarioEmail"] = EmpresaDados.Email;
                this.Session["Email"] = EmpresaDados.Email;

                return RedirectToAction("../Dashboard/Dashboard");
            }
            else
            {
                ViewBag.Error = "E-mail ou senha incorretos.";
            }
            return View(new Empresa());
        }


        // POST: NovaConta
        [HttpPost]
        public ActionResult NovaConta(Empresa EmpresaModel)
        {
            if (ModelState.IsValid)
            {
                if (dao.VerificaEmpresa(EmpresaModel.Email))
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
                else
                {
                    ViewBag.Error = "A empresa " + EmpresaModel.RazaoSocial + " já possui um cadastro.";
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