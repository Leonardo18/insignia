﻿using System.Web.Mvc;
using System.Web.Routing;

namespace Insignia.Painel
{
    public class RouteConfig
    {
        /// <summary>
        /// Monta as rotas de acesso
        /// </summary>
        /// <param name="routes">Objeto que crias as rotas</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                 name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}

