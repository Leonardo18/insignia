﻿using Insignia.DAO.Autenticacao;
using Insignia.DAO.Util;
using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;

namespace Insignia.Painel.Helpers.CustomAttributes
{
    public class HavePermission : ActionFilterAttribute, IActionFilter
    {
        public string AreaNome { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = HttpContext.Current;

            if (!string.IsNullOrWhiteSpace(AreaNome))
            {
                Auth auth = new Auth(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);
                string usuarioID = Convert.ToString(context.Session["UsuarioID"]);
                string tipo = Convert.ToString(context.Session["UsuarioTipo"]) != "Empresa" ? Database.DBBuscaInfo("Usuarios", "ID", Convert.ToString(context.Session["UsuarioID"]), "Tipo") : "Empresa";

                if (!string.IsNullOrEmpty(tipo))
                {
                    switch (tipo)
                    {
                        case "Empresa":
                            if (AreaNome == "DashboardGestor")
                                context.Response.Redirect("~/Areas/Permissao");
                            if (AreaNome == "DashboardFuncionario")
                                context.Response.Redirect("~/Areas/Permissao");
                            break;

                        case "Gestor":
                            if (AreaNome == "Setores")
                                context.Response.Redirect("~/Areas/Permissao");
                            if (AreaNome == "DashboardEmpresa")
                                context.Response.Redirect("~/Areas/Permissao");
                            if (AreaNome == "DashboardFuncionario")
                                context.Response.Redirect("~/Areas/Permissao");
                            break;

                        case "Funcionario":
                            if (AreaNome == "Badges")
                                context.Response.Redirect("~/Areas/Permissao");
                            if (AreaNome == "Competencias")
                                context.Response.Redirect("~/Areas/Permissao");
                            if (AreaNome == "Setores")
                                context.Response.Redirect("~/Areas/Permissao");
                            if (AreaNome == "Usuarios")
                                context.Response.Redirect("~/Areas/Permissao");
                            if (AreaNome == "DashboardEmpresa")
                                context.Response.Redirect("~/Areas/Permissao");
                            if (AreaNome == "DashboardGestor")
                                context.Response.Redirect("~/Areas/Permissao");
                            break;
                    }
                }
            }
        }
    }
}