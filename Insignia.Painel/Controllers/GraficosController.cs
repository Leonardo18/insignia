using Insignia.Painel.Helpers.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class GraficosController : Controller
    {
        /// <summary>
        /// GET: Gráfico Badges
        /// </summary>
        /// <returns>Retorna a view de gráfico das badges</returns>
        [HttpGet, IsLogged]
        public ActionResult Badges()
        {
            //Filtros de usuários e setores - Criar


            return View("Badges");
        }

        /// <summary>
        /// POST: Gráfico Badges
        /// </summary>
        /// <param name="FiltroSetor">ID do setor que será filtrado</param>
        /// <param name="FiltroUsuario">ID do usuário que será filtrado</param>
        /// <returns>Retorna a view com novos dados no model conforme filtros passados</returns>
        [HttpPost, IsLogged]
        public ActionResult Badges(int FiltroSetor, int FiltroUsuario)
        {
            //Filtros de usuários e setores - Criar


            return View("Badges");
        }
    }
}