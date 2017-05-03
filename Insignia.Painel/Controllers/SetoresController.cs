using Insignia.DAO.Setores;
using Insignia.Model.Setor;
using Insignia.Painel.Helpers.CustomAttributes;
using System.Configuration;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class SetoresController : Controller
    {
        private SetoresDAO SetoresDAO = new SetoresDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);


        /// <summary>
        /// GET: Setores Listar
        /// </summary>
        /// <returns>Retorna a view de listar setores com os dados</returns>
        [HttpGet, IsLogged, HavePermission(AreaNome = "Setores")]
        public ActionResult Listar()
        {
            var SetorModel = SetoresDAO.Listar();

            return View(SetorModel);
        }

        /// <summary>
        /// GET: Setores Adicionar
        /// </summary>
        /// <returns>Retorna a view de adicionar setores</returns>
        [HttpGet, IsLogged, HavePermission(AreaNome = "Setores")]
        public ActionResult Adicionar()
        {
            var SetorModel = new Setor();

            return View(SetorModel);
        }

        /// <summary>
        /// POST: Setores Adicionar 
        /// </summary>
        /// <param name="SetorModel">Objeto Model do Setor contendo os dados inseridos para cadastro</param>
        /// <returns>Caso consiga validar e salvar o setor faz redirecionamento, se não retorna a view com mensagem</returns>
        [HttpPost, IsLogged, HavePermission(AreaNome = "Setores")]
        public ActionResult Adicionar(Setor SetorModel)
        {
            if (ModelState.IsValid)
            {
                if (SetoresDAO.Salvar(SetorModel))
                {
                    return RedirectToAction("Editar", new { ID = SetorModel.ID });
                }
            }

            return View(SetorModel);
        }

        /// <summary>
        /// GET: Setor Editar 
        /// </summary>
        /// <param name="ID">ID do Setor a ser editado</param>
        /// <returns>Retorna a view com os dados do setror a serem editados</returns>
        [HttpGet, IsLogged, HavePermission(AreaNome = "Setores")]
        public ActionResult Editar(int ID)
        {
            Setor SetorModel = SetoresDAO.Carregar(ID);

            return View("Editar", SetorModel);
        }

        /// <summary>
        /// POST: Setor Editar
        /// </summary>
        /// <param name="SetorModel">Model contendo os dados do Setor</param>
        /// <returns>Caso consiga validar os dados e atualizar o setor faz redirecionamento, caso contrário retorna a view novamente para ajuste de dados inválidos</returns>
        [HttpPost, IsLogged, HavePermission(AreaNome = "Setores")]
        public ActionResult Editar(Setor SetorModel)
        {
            if (ModelState.IsValid)
            {
                if (SetoresDAO.Editar(SetorModel))
                {
                    return RedirectToAction("Editar", new { ID = SetorModel.ID });
                }
            }

            return View("Editar", SetorModel);
        }

        /// <summary>
        /// GET: Setor Remover
        /// </summary>
        /// <param name="ID">ID do setor a ser removido</param>
        /// <returns>Retorna a view com dados do setor que será removido</returns>
        [HttpGet, IsLogged, HavePermission(AreaNome = "Setores")]
        public ActionResult Remover(int ID)
        {
            //Faz Load com o ID passado
            Setor SetorModel = SetoresDAO.Carregar(ID);

            return View(SetorModel);
        }

        /// <summary>
        /// POST: Setor Remover
        /// </summary>
        /// <param name="SetorModel">Model contendo os dados do setor</param>
        /// <returns>Caso consiga remover o setor do sistema faz redirecionamento, caso contrário retorna a view com mensagem</returns>
        [HttpPost, IsLogged, HavePermission(AreaNome = "Setores")]
        public ActionResult Remover(Setor SetorModel)
        {
            //Faz Load com o ID passado
            SetorModel = SetoresDAO.Carregar(SetorModel.ID);

            if (SetorModel != null)
            {
                if (!SetoresDAO.PodeRemover(SetorModel.ID))
                {
                    if (SetoresDAO.Remover(SetorModel.ID))
                    {
                        return RedirectToAction("Listar");
                    }
                    else
                    {
                        ViewBag.Error = "Ocorreu um erro ao tentar excluir o resgistro, favor entrar em contato com o administrador do sistema";
                    }
                }
                else
                {
                    ViewBag.Error = "Não é possível remover o setor " + SetorModel.Nome + ", pois existem badges cadastras para o mesmo.";
                }
            }

            return View(SetorModel);
        }
    }
}