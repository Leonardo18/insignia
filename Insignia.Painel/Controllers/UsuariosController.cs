using Insignia.DAO.Competencias;
using Insignia.DAO.Tarefas;
using Insignia.DAO.Usuarios;
using Insignia.DAO.Util;
using Insignia.Model.Usuario;
using Insignia.Painel.Helpers.AmazonS3;
using Insignia.Painel.Helpers.CustomAttributes;
using Insignia.Painel.Helpers.Util;
using Insignia.Painel.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
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
                item.SetorNome = Database.DBBuscaInfo("Setores", "ID", Convert.ToString(item.SetorID), "Nome");
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

            //Busca os tipos de tarefa e retorna um dictionary contendo os dados e retorna o select list
            ViewBag.Setores = SelectListMVC.CriaListaSelecao(UsuariosDAO.Setores());

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
                if (UsuariosDAO.VerificaUsuario(0, UsuarioModel.Email) && string.IsNullOrEmpty(Database.DBBuscaInfo("Empresas", "Email", UsuarioModel.Email, "ID")))
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

            //Busca os  e retorna um dictionary contendo os dados
            var UsuariosTipos = SelectListMVC.CriaListaSelecao(UsuariosDAO.Setores());

            foreach (var item in UsuariosTipos)
            {
                if (item.Value == UsuarioModel.Tipo)
                {
                    item.Selected = true;
                    break;
                }
            }

            ViewBag.Setores = UsuariosTipos;

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

            //Busca os tipos de tarefa e retorna um dictionary contendo elas
            var UsuariosTipos = SelectListMVC.CriaListaSelecao(UsuariosDAO.Setores());

            //Retorna na list o valor marcado atualmente para o cadastro
            foreach (var item in UsuariosTipos)
            {
                if (item.Value == Convert.ToString(UsuarioModel.SetorID))
                {
                    item.Selected = true;
                    break;
                }
            }

            ViewBag.Setores = UsuariosTipos;

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
                if (UsuariosDAO.VerificaUsuario(UsuarioModel.ID, UsuarioModel.Email) && string.IsNullOrEmpty(Database.DBBuscaInfo("Empresas", "Email", UsuarioModel.Email, "ID")))
                {
                    if (UsuariosDAO.Editar(UsuarioModel))
                    {
                        return RedirectToAction("Editar", new { ID = UsuarioModel.ID });
                    }
                }
            }

            //Busca os tipos de tarefa e retorna um dictionary contendo elas
            var UsuariosTipos = SelectListMVC.CriaListaSelecao(UsuariosDAO.Setores());

            //Retorna na list o valor marcado atualmente para o cadastro
            foreach (var item in UsuariosTipos)
            {
                if (item.Value == Convert.ToString(UsuarioModel.SetorID))
                {
                    item.Selected = true;
                    break;
                }
            }

            ViewBag.Setores = UsuariosTipos;

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
                else
                {
                    ViewBag.Error = "Ocorreu um erro ao tentar excluir o resgistro, favor entrar em contato com o administrador do sistema";
                }
            }

            return View(UsuarioModel);
        }

        /// <summary>
        /// GET: Usuário Perfil
        /// </summary>        
        /// <returns>Retorna a view com os dados do usuário</returns>
        [HttpGet, IsLogged]
        public ActionResult Perfil(int id)
        {
            var ViewModel = new ViewModelPerfil();
            ViewModel.Usuario = UsuariosDAO.Carregar(id);

            TarefasDAO TarefasDAO = new TarefasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);
            //Busca as tarefa com status finalizada
            ViewModel.ListFinalizadas = TarefasDAO.ListarTop(ConfigurationManager.AppSettings["Finalizada"], 0, 5);

            ViewModel.TarefasMes = new List<int>();

            for (int i = 1; i <= 12; i++)
            {
                ViewModel.TarefasMes.Add(TarefasDAO.QuantidadeTarefasMes(i));
            }

            CompetenciasDAO CompetenciasDAO = new CompetenciasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);
            ViewModel.ListCompetencias = CompetenciasDAO.Listar();

            foreach (var item in ViewModel.ListCompetencias)
            {
                item.Pontos = CompetenciasDAO.CompetenciaPontos(item.ID);
            }

            return View(ViewModel);
        }

        /// <summary>
        /// GET: Usuário PerfilEditar
        /// </summary>        
        /// <returns>Retorna a view com os dados do usuário para edição</returns>
        [HttpGet, IsLogged]
        public ActionResult PerfilEditar(int id)
        {
            var UsuarioModel = UsuariosDAO.Carregar(id);

            List<SelectListItem> Estados = new List<SelectListItem>();

            //Busca os  e retorna um dictionary contendo os dados
            var TodosEstados = UsuariosDAO.Estados();

            foreach (var item in TodosEstados.Keys)
            {
                Estados.Add(new SelectListItem { Text = TodosEstados[item], Value = Convert.ToString(item) });
            }

            //Retorna na list o valor marcado atualmente para o cadastro
            foreach (var item in Estados)
            {
                if (item.Text == UsuarioModel.Estado)
                {
                    item.Selected = true;
                    break;
                }
            }
            ViewBag.Estados = Estados;

            return View(UsuarioModel);
        }

        /// <summary>
        /// POST: Usuário PerfilEditar
        /// </summary>        
        /// <returns>Retorna a view com os dados do usuário para edição</returns>
        [HttpPost, IsLogged]
        public ActionResult PerfilEditar(Usuario UsuarioModel, HttpPostedFileBase Foto)
        {
            // Verifica se existe um arquivo escolhido
            if (Foto != null && Foto.ContentLength > 0)
            {
                AmazonUpload AmazonS3 = new AmazonUpload();

                //Verifica se possui arquivo antigo para substituição na amazon
                string ArquivoAntigo = Database.DBBuscaInfo("Usuarios", "ID", Convert.ToString(UsuarioModel.ID), "Foto");

                // Pega o nome do arquivo
                UsuarioModel.Foto = Path.GetFileName(Foto.FileName);

                // Grava o arquivo em uma pasta local
                var Caminho = Path.Combine(Server.MapPath("~/Content/uploads"), UsuarioModel.Foto);
                Foto.SaveAs(Caminho);

                //Verifica se existe a pasta da empresa no Bucket
                if (!AmazonS3.ExistePasta(Convert.ToString(Session["EmpresaNome"]), "Fotos", ConfigurationManager.AppSettings["BucketName"]))
                {
                    //Cria uma pasta no Bucket com o nome da empresa
                    AmazonS3.CriaPasta(Convert.ToString(Session["EmpresaNome"]), "Fotos", ConfigurationManager.AppSettings["BucketName"]);
                }

                //Apaga arquivo antigo para fazer upload de um novo
                AmazonS3.ApagaArquivo(ConfigurationManager.AppSettings["BucketName"], Convert.ToString(Session["EmpresaNome"]), "Fotos", ArquivoAntigo);

                //Faz Upload do arquivo para o S3
                AmazonS3.EnviaArquivoS3(Caminho, ConfigurationManager.AppSettings["BucketName"], Convert.ToString(Session["EmpresaNome"]), "Fotos", UsuarioModel.Foto);

                //Deleta arquivo salvo local
                System.IO.File.Delete(Caminho);
            }
            else
            {
                //Se não tem arquivo nome, mantém o antigo
                UsuarioModel.Foto = Database.DBBuscaInfo("Usuarios", "ID", Convert.ToString(UsuarioModel.ID), "Foto");
            }

            if (UsuariosDAO.EditarPerfil(UsuarioModel))
            {
                Session["UsuarioFoto"] = UsuarioModel.Foto;

                return RedirectToAction("Perfil", new { ID = UsuarioModel.ID });
            }

            List<SelectListItem> Estados = new List<SelectListItem>();

            //Busca e retorna um dictionary contendo os dados
            var TodosEstados = UsuariosDAO.Estados();

            foreach (var item in TodosEstados.Keys)
            {
                Estados.Add(new SelectListItem { Text = TodosEstados[item], Value = Convert.ToString(item) });
            }

            ViewBag.Estados = Estados;

            return RedirectToAction("PerfilEditar", UsuarioModel);
        }
    }
}