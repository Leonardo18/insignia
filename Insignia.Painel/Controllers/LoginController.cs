using Insignia.DAO.Autenticacao;
using Insignia.DAO.Empresas;
using Insignia.DAO.Usuarios;
using Insignia.DAO.Util;
using Insignia.Model.Empresa;
using Insignia.Model.Usuario;
using Insignia.Painel.Helpers.Email;
using System;
using System.Configuration;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class LoginController : Controller
    {
        private EmpresasDAO EmpresaDAO = new EmpresasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);
        private UsuariosDAO UsuarioDAO = new UsuariosDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

        /// <summary>
        /// GET: Login 
        /// </summary>
        /// <returns>Retorna a view de login</returns>
        [HttpGet]
        public ActionResult Login()
        {
            if (!string.IsNullOrEmpty(Convert.ToString(Session["Error"])))
            {
                ViewBag.Success = string.Empty;
                ViewBag.Error = Convert.ToString(Session["Error"]);
            }

            if (!string.IsNullOrEmpty(Convert.ToString(Session["Success"])))
            {
                ViewBag.Error = string.Empty;
                ViewBag.Success = Convert.ToString(Session["Success"]);
            }

            return View(new Empresa());
        }

        /// <summary>
        /// POST: Login 
        /// </summary>
        /// <param name="Email">Email de acesso</param>
        /// <param name="Senha">Senha de acesso</param>
        /// <returns>Caso consiga fazer login com os dados informados redireciona, caso contrário retorna a view com mensagem</returns>
        [HttpPost]
        public ActionResult Login(string Email, string Senha)
        {
            //Objeto dao da dll que contém os métodos para buscar dados e editar dados no banco.
            Auth auth = new Auth(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

            //Tenta efeutar login com os dados passados e retorna um dictionary
            Empresa EmpresaModel = auth.LoginEmpresa(Email, Senha);

            Usuario UsuarioModel = auth.LoginUsuario(Email, Senha);

            if (EmpresaModel != null)
            {
                Session["SessionID"] = Session.SessionID;
                Session["EmpresaID"] = EmpresaModel.ID;
                Session["EmpresaNome"] = EmpresaModel.RazaoSocial;
                Session["UsuarioID"] = EmpresaModel.ID;
                Session["UsuarioNome"] = EmpresaModel.RazaoSocial;
                Session["UsuarioEmail"] = EmpresaModel.Email;
                Session["UsuarioFoto"] = EmpresaModel.Foto;

                return RedirectToAction("../Dashboard/Dashboard");
            }
            else if (UsuarioModel != null)
            {
                Session["SessionID"] = Session.SessionID;
                Session["EmpresaID"] = UsuarioModel.EmpresaID;
                Session["EmpresaNome"] = Database.DBBuscaInfo("Empresas", "ID", Convert.ToString(UsuarioModel.EmpresaID), "RazaoSocial");
                Session["SetorID"] = UsuarioModel.SetorID;
                Session["UsuarioID"] = UsuarioModel.ID;
                Session["UsuarioNome"] = UsuarioModel.Nome;
                Session["UsuarioEmail"] = UsuarioModel.Email;
                Session["UsuarioFoto"] = UsuarioModel.Foto;
                Session["UsuarioTipo"] = UsuarioModel.Tipo;

                return RedirectToAction("../Dashboard/Dashboard");
            }
            else
            {
                ViewBag.Error = "E-mail ou senha incorretos.";
            }

            return View(new Empresa());
        }

        /// <summary>
        /// POST: NovaConta 
        /// </summary>
        /// <param name="EmpresaModel">Model contendo os dados da Empresa</param>
        /// <returns>Caso consiga validar os dados e salvar o cadastro, redireciona, caso contrário retorna a view com menssagem</returns>
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
                        Session["EmpresaNome"] = EmpresaModel.RazaoSocial;
                        Session["UsuarioID"] = EmpresaModel.ID;
                        Session["UsuarioNome"] = EmpresaModel.RazaoSocial;
                        Session["UsuarioEmail"] = EmpresaModel.Email;

                        SendMail Email = new SendMail();

                        if (Email.EnviaEmail(EmpresaModel.RazaoSocial, EmpresaModel.Email, "Você efetuou um cadatrado no sistema Insígnia.", "NovoCadastro.html"))
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

        /// <summary>
        /// POST: RecuperarSenha 
        /// </summary>
        /// <param name="email">Email cadastrado no sistema</param>
        [HttpPost]
        public ActionResult RecuperarSenha(string email)
        {
            if (EmpresaDAO.VerificaEmpresa(email, null) || UsuarioDAO.VerificaUsuario(email))
            {
                SendMail Email = new SendMail();

                if (Email.EnviaEmail(email, email, "Foi solicitado uma recuperação de senha no sistema Insígnia.", "RecuperarSenha.html"))
                {
                    Session["Success"] = "Foi enviado um e-mail para " + email + ", verifique o e-mail informado para redefinir sua senha.";
                }
            }
            else
            {
                Session["Error"] = "O e-mail informado não existe no sistema insígnia.";
            }

            return RedirectToAction("Login");
        }

        /// <summary>
        /// GET: ResetarSenha 
        /// </summary>
        /// <param name="email">Email cadastrado no sistema e para recuperação de senha</param>
        [HttpGet]
        public ActionResult ResetarSenha(string email)
        {
            //Salvo o e-mail criptografado em uma session
            Session["EmailRecuperacao"] = email;

            return View();
        }

        /// <summary>
        /// POST: ResetarSenha 
        /// </summary>        
        /// <param name="senhaCadastro">Nova senha do usuário</param>
        /// <param name="confirmaSenha">Confirmação da nova senha do usuário</param>
        [HttpPost]
        public ActionResult ResetarSenha(string senhaCadastro, string confirmaSenha)
        {
            string email = Convert.ToString(Session["EmailRecuperacao"]);

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(senhaCadastro) && !string.IsNullOrEmpty(confirmaSenha))
            {
                if (senhaCadastro == confirmaSenha)
                {
                    Utilitarios Util = new Utilitarios();

                    email = Util.Descriptografar(email);

                    if (EmpresaDAO.AtualizaSenha(email, senhaCadastro))
                    {
                        return RedirectToAction("Sair");
                    }
                    else if (UsuarioDAO.AtualizaSenha(email, senhaCadastro))
                    {
                        return RedirectToAction("Sair");
                    }
                    else
                    {
                        ViewBag.Error = "Ocorreu um problema ao tentar atualizar a senha, verifique o e-mail informado para recuperação de senha ou entre em contato com o administrador do sistema.";
                    }
                }
                else
                {
                    ViewBag.Error = "As senhas digitadas diferem.";
                }
            }

            return View();
        }

        /// <summary>
        /// GET: Logout 
        /// </summary>
        /// <returns>Limpa a sessão de acesso e faz redirecionamento</returns>
        [HttpGet]
        public ActionResult Sair()
        {
            Session.Clear();

            return RedirectToAction("Login");
        }
    }
}