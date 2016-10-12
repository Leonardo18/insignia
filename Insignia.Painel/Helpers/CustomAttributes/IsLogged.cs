﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Insignia.Painel.Helpers.CustomAttributes
{
    public class IsLogged : ActionFilterAttribute, IActionFilter
    {
        /// <summary>
        /// Verifica se uusário possui sessão existente se não tiver redireciona par ao login
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Convert.ToString(HttpContext.Current.Session["SessionID"]) != HttpContext.Current.Session.SessionID || string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["UsuarioID"])))
                HttpContext.Current.Response.Redirect("~/Login");
        }
    }
}