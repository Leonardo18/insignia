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
        [HttpGet, IsLogged]
        public ActionResult Listar()
        {
            var SetorModel = SetoresDAO.Listar();

            return View(SetorModel);
        }

        /// <summary>
        /// GET: Setores Adicionar
        /// </summary>
        /// <returns>Retorna a view de adicionar setores</returns>
        [HttpGet, IsLogged]
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
        [HttpPost, IsLogged]
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
        [HttpGet, IsLogged]
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
        [HttpPost, IsLogged]
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
        [HttpGet, IsLogged]
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
        [HttpPost, IsLogged]
        public ActionResult Remover(Setor SetorModel)
        {
            //Faz Load com o ID passado
            SetorModel = SetoresDAO.Carregar(SetorModel.ID);

            if (SetorModel != null)
            {
                if (SetoresDAO.Remover(SetorModel.ID))
                {
                    return RedirectToAction("Listar");
                }
            }

            return View(SetorModel);
        }
    }
}