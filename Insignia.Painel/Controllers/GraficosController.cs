using Insignia.DAO.Graficos;
using Insignia.Painel.Helpers.CustomAttributes;
using Insignia.Painel.Helpers.Util;
using Insignia.Painel.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class GraficosController : Controller
    {
        private GraficosDAO GraficosDAO = new GraficosDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

        /// <summary>
        /// GET: Gráfico Badges
        /// </summary>
        /// <returns>Retorna a view de gráfico das badges</returns>
        [HttpGet, IsLogged]
        public ActionResult Badges()
        {
            var ViewModel = new ViewModelGraficos();

            //Busca todos os setores e retorna um dictionary contendo os dados e retorna o select list
            ViewBag.Setores = SelectListMVC.CriaListaSelecao(GraficosDAO.Setores());

            //Busca todos os usuários e retorna um dictionary contendo os dados e retorna o select list
            ViewBag.Usuarios = SelectListMVC.CriaListaSelecao(GraficosDAO.Usuarios());

            return View(ViewModel);
        }

        /// <summary>
        /// POST: Gráfico Badges
        /// </summary>
        /// <param name="FiltroSetor">ID do setor que será filtrado</param>
        /// <param name="FiltroUsuario">ID do usuário que será filtrado</param>
        /// <returns>Retorna a view com novos dados no model conforme filtros passados</returns>
        [HttpPost, IsLogged]
        public ActionResult Badges(int FiltroSetor = 0, int FiltroUsuario = 0)
        {
            var ViewModel = new ViewModelGraficos();

            GraficosDAO.Badge(FiltroSetor, FiltroUsuario);

            var Setores = SelectListMVC.CriaListaSelecao(GraficosDAO.Setores());

            foreach (var item in Setores)
            {
                if (Convert.ToInt32(item.Value) == FiltroSetor)
                {
                    item.Selected = true;
                    break;
                }
            }

            ViewBag.Setores = Setores;

            var Usuarios = SelectListMVC.CriaListaSelecao(GraficosDAO.Usuarios());

            foreach (var item in Setores)
            {
                if (Convert.ToInt32(item.Value) == FiltroUsuario)
                {
                    item.Selected = true;
                    break;
                }
            }

            ViewBag.Usuarios = Usuarios;

            return View(ViewModel);
        }
    }
}