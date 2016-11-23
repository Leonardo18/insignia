using Insignia.DAO.Badges;
using Insignia.Model.Badge;
using Insignia.Painel.Helpers.CustomAttributes;
using Insignia.Painel.Helpers.Util;
using Insignia.Painel.ViewModels;
using System.Configuration;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class BadgesController : Controller
    {
        private BadgesDAO BadgesDAO = new BadgesDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

        /// <summary>
        /// GET: Badge Adicionar
        /// </summary>
        /// <returns>Retorna a view de adicionar badge</returns>
        [HttpGet, IsLogged]
        public ActionResult Adicionar()
        {
            var ViewModel = new ViewModelBadge();

            ViewModel.Badge = new Badge();
            ViewModel.ListBadge = BadgesDAO.Listar();

            return View(ViewModel);
        }

        /// <summary>
        /// POST: Badge Adicionar 
        /// </summary>
        /// <param name="BadgeModel">Objeto Model da badge contendo os dados inseridos para cadastro</param>
        /// <returns>Caso consiga validar os dados e salvar a badge faz redirecionamento, caso contrário retorna a view novamente para ajuste de dados inválidos</returns>
        [HttpPost, IsLogged]
        public ActionResult Adicionar(Badge BadgeModel)
        {
            //Objeto com funções de cores
            BadgesCor cor = new BadgesCor();

            var ViewModel = new ViewModelBadge();

            if (ModelState.IsValid)
            {
                BadgeModel.CorFonte = cor.HexToColor(BadgeModel.Cor);

                if (BadgesDAO.Salvar(BadgeModel))
                {
                    return RedirectToAction("../Badges/Adicionar");
                }
            }

            ViewModel.Badge = new Badge();
            ViewModel.ListBadge = BadgesDAO.Listar();

            return View(ViewModel);
        }

        /// <summary>
        /// GET: Badge Editar
        /// </summary>
        /// <param name="ID">ID da badge que será editada</param>
        /// <returns>Retorna a view com os dados da badge a serem editados</returns>
        [HttpGet, IsLogged]
        public ActionResult Editar(int ID)
        {
            Badge BadgeModel = BadgesDAO.Carregar(ID);

            return View("Editar", BadgeModel);
        }

        /// <summary>
        /// POST: Badge Editar 
        /// </summary>
        /// <param name="BadgeModel">Objeto Model da badge contendo os dados atualiados</param>
        /// <returns>Caso consiga validar os dados e atualizar a badge faz redirecionamento, caso contrário retorna a view novamente para ajuste de dados inválidos</returns>
        [HttpPost, IsLogged]
        public ActionResult Editar(Badge BadgeModel)
        {
            //Objeto com funções de cores
            BadgesCor cor = new BadgesCor();

            if (ModelState.IsValid)
            {
                BadgeModel.CorFonte = cor.HexToColor(BadgeModel.Cor);

                if (BadgesDAO.Editar(BadgeModel))
                {
                    return RedirectToAction("Adicionar");
                }
            }
            return View("Editar", BadgeModel);
        }

        /// <summary>
        /// GET: Badge Remover 
        /// </summary>
        /// <param name="ID">ID da bagde a ser removida</param>
        /// <returns>Retorna a view com dados da badge que será removida</returns>
        [HttpGet, IsLogged]
        public ActionResult Remover(int ID)
        {
            //Faz Load com o ID passado
            Badge BadgeModel = BadgesDAO.Carregar(ID);

            return View(BadgeModel);
        }

        /// <summary>
        /// POST: Badge Remover
        /// </summary>
        /// <param name="BadgeModel">Model da badge contendo dados dela</param>
        /// <returns>Caso consiga remover a badge do sistema faz redirecionamento, caso contrário retorna a view com mensagem</returns>
        [HttpPost, IsLogged]
        public ActionResult Remover(Badge BadgeModel)
        {
            //Faz Load com o ID passado
            BadgeModel = BadgesDAO.Carregar(BadgeModel.ID);

            if (BadgeModel != null)
            {
                if (!BadgesDAO.PodeRemover(BadgeModel.ID))
                {
                    if (BadgesDAO.Remover(BadgeModel.ID))
                    {
                        return RedirectToAction("Adicionar");
                    }
                }
                else
                {
                    ViewBag.Error = "Não é possível remover a badge " + BadgeModel.Titulo + ", pois existem tarefas cadastras para a mesma.";
                }
            }
            return View(BadgeModel);
        }
    }
}