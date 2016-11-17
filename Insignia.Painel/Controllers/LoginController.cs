using Insignia.DAO.Autenticacao;
using Insignia.DAO.Empresas;
using Insignia.Model.Empresa;
using Insignia.Painel.Helpers.Email;
using System.Configuration;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class LoginController : Controller
    {
        private EmpresasDAO EmpresaDAO = new EmpresasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

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
            Empresa EmpresaModel = auth.LoginEmpresa(Email, Senha);

            if (EmpresaModel != null)
            {
                Session["SessionID"] = Session.SessionID;
                Session["EmpresaID"] = EmpresaModel.ID;
                Session["UsuarioID"] = EmpresaModel.ID;
                Session["UsuarioNome"] = EmpresaModel.RazaoSocial;
                Session["UsuarioEmail"] = EmpresaModel.Email;

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
                if (EmpresaDAO.VerificaEmpresa(EmpresaModel.Email, EmpresaModel.CNPJ))
                {
                    if (EmpresaDAO.Salvar(EmpresaModel))
                    {
                        Session["SessionID"] = Session.SessionID;
                        Session["EmpresaID"] = EmpresaModel.ID;
                        Session["UsuarioID"] = EmpresaModel.ID;
                        Session["UsuarioNome"] = EmpresaModel.RazaoSocial;
                        Session["UsuarioEmail"] = EmpresaModel.Email;

                        SendMail Email = new SendMail();

                        if (Email.EnviaEmail(EmpresaModel.RazaoSocial, EmpresaModel.Email, "Você efetuou um cadatrado no sistema Insígnia."))
                        {
                            return RedirectToAction("../Dashboard/Dashboard");
                        }
                        else
                        {
                            ViewBag.Error = "Não foi possível enviar um e-mail de validação para: " + EmpresaModel.Email + ", verifique o e-mail informado no cadastro.";
                            EmpresaDAO.Remover(EmpresaModel.ID);
                        }
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
            Session.Clear();

            return RedirectToAction("Login");
        }
    }
}