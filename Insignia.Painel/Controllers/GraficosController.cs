﻿using Insignia.DAO.Graficos;
using Insignia.Painel.Helpers.CustomAttributes;
using Insignia.Painel.Helpers.Util;
using Insignia.Painel.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            var ViewModel = new ViewModelGraficoBadges();

            if (Convert.ToString(Session["UsuarioTipo"]) == "Empresa")
            {
                //Busca todos os setores e retorna um dictionary contendo os dados e retorna o select list
                ViewBag.Setores = SelectListMVC.CriaListaSelecao(GraficosDAO.Setores());
            }

            int usuarioID = 0;

            //Busca todos os usuários e retorna um dictionary contendo os dados e retorna o select list            
            var Usuarios = SelectListMVC.CriaListaSelecao(GraficosDAO.Usuarios(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : 0));

            foreach (var item in Usuarios)
            {
                if (Convert.ToInt32(item.Value) == Convert.ToInt32(Session["UsuarioID"]))
                {
                    item.Selected = true;
                    usuarioID = Convert.ToInt32(item.Value);
                    break;
                }
            }

            ViewBag.Usuarios = Usuarios;

            ViewModel.ListBadgeBasicas = GraficosDAO.Badges("Basica", !string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : 0);
            ViewModel.ListBadgeIntermediarias = GraficosDAO.Badges("Intermediaria", !string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : 0);
            ViewModel.ListBadgeAvancadas = GraficosDAO.Badges("Avancada", !string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : 0);

            //Busca o total de usuário da empresa, caso o usuário logado seja gestor ou funcionário busca o total de usuários do setor correspondente
            ViewModel.TotalUsuarios = GraficosDAO.TotalUsuarios(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : 0);

            foreach (var item in ViewModel.ListBadgeBasicas)
            {
                ViewModel.TotalBadgesAdquiridas = GraficosDAO.BadgeAdquiridas(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : 0, usuarioID, item.ID);

                if (ViewModel.TotalBadgesAdquiridas != 0)
                {
                    item.Adquirida = true;
                    item.Progresso = Math.Round(((double)ViewModel.TotalBadgesAdquiridas / ViewModel.TotalUsuarios) * 100, 0);
                }
                else
                {
                    if (usuarioID == 0)
                    {
                        item.Adquirida = true;
                    }
                }
            }

            foreach (var item in ViewModel.ListBadgeIntermediarias)
            {
                ViewModel.TotalBadgesAdquiridas = GraficosDAO.BadgeAdquiridas(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : 0, usuarioID, item.ID);

                if (ViewModel.TotalBadgesAdquiridas != 0)
                {
                    item.Adquirida = true;
                    item.Progresso = Math.Round(((double)ViewModel.TotalBadgesAdquiridas / ViewModel.TotalUsuarios) * 100, 0);
                }
                else
                {
                    if (usuarioID == 0)
                    {
                        item.Adquirida = true;
                    }
                }
            }

            foreach (var item in ViewModel.ListBadgeAvancadas)
            {
                ViewModel.TotalBadgesAdquiridas = GraficosDAO.BadgeAdquiridas(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : 0, usuarioID, item.ID);

                if (ViewModel.TotalBadgesAdquiridas != 0)
                {
                    item.Adquirida = true;
                    item.Progresso = Math.Round(((double)ViewModel.TotalBadgesAdquiridas / ViewModel.TotalUsuarios) * 100, 0);
                }
                else
                {
                    if (usuarioID == 0)
                    {
                        item.Adquirida = true;
                    }
                }
            }

            ViewBag.UsuarioID = usuarioID;

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
            var ViewModel = new ViewModelGraficoBadges();

            ViewModel.ListBadgeBasicas = GraficosDAO.Badges("Basica", !string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor);
            ViewModel.ListBadgeIntermediarias = GraficosDAO.Badges("Intermediaria", !string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor);
            ViewModel.ListBadgeAvancadas = GraficosDAO.Badges("Avancada", !string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor);

            //Busca o total de usuário da empresa, caso o usuário logado seja gestor ou funcionário busca o total de usuários do setor correspondente
            ViewModel.TotalUsuarios = GraficosDAO.TotalUsuarios(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor);

            foreach (var item in ViewModel.ListBadgeBasicas)
            {
                ViewModel.TotalBadgesAdquiridas = GraficosDAO.BadgeAdquiridas(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor, FiltroUsuario, item.ID);

                if (ViewModel.TotalBadgesAdquiridas != 0)
                {
                    item.Adquirida = true;
                    item.Progresso = Math.Round(((double)ViewModel.TotalBadgesAdquiridas / ViewModel.TotalUsuarios) * 100, 0);
                }
                else
                {
                    if (FiltroUsuario == 0)
                    {
                        item.Adquirida = true;
                    }
                }
            }

            foreach (var item in ViewModel.ListBadgeIntermediarias)
            {
                ViewModel.TotalBadgesAdquiridas = GraficosDAO.BadgeAdquiridas(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor, FiltroUsuario, item.ID);

                if (ViewModel.TotalBadgesAdquiridas != 0)
                {
                    item.Adquirida = true;                    
                    item.Progresso = Math.Round(((double)ViewModel.TotalBadgesAdquiridas / ViewModel.TotalUsuarios) * 100, 0);
                }
                else
                {
                    if (FiltroUsuario == 0)
                    {
                        item.Adquirida = true;
                    }
                }
            }

            foreach (var item in ViewModel.ListBadgeAvancadas)
            {
                ViewModel.TotalBadgesAdquiridas = GraficosDAO.BadgeAdquiridas(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor, FiltroUsuario, item.ID);

                if (ViewModel.TotalBadgesAdquiridas != 0)
                {
                    item.Adquirida = true;
                    item.Progresso = Math.Round(((double)ViewModel.TotalBadgesAdquiridas / ViewModel.TotalUsuarios) * 100, 0);
                }
                else
                {
                    if (FiltroUsuario == 0)
                    {
                        item.Adquirida = true;
                    }
                }
            }

            ViewBag.UsuarioID = FiltroUsuario;

            //Recarrega o dropdownlist de setores setando o valor que havia sido usado como filtro
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

            //Recarrega o dropdownlist de usuários setando o valor que havia sido usado como filtro
            var Usuarios = SelectListMVC.CriaListaSelecao(GraficosDAO.Usuarios(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor));

            foreach (var item in Usuarios)
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

        /// <summary>
        /// GET: Gráfico Tarefas
        /// </summary>
        /// <returns>Retorna a view de gráfico das tarefas</returns>
        [HttpGet, IsLogged]
        public ActionResult Tarefas()
        {
            var ViewModel = new ViewModelGraficoTarefas();

            if (Convert.ToString(Session["UsuarioTipo"]) == "Empresa")
            {
                //Busca todos os setores e retorna um dictionary contendo os dados e retorna o select list
                ViewBag.Setores = SelectListMVC.CriaListaSelecao(GraficosDAO.Setores());
            }

            int UsuarioID = 0;

            //Busca todos os usuários e retorna um dictionary contendo os dados e retorna o select list            
            var Usuarios = SelectListMVC.CriaListaSelecao(GraficosDAO.Usuarios(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : 0));

            foreach (var item in Usuarios)
            {
                if (Convert.ToInt32(item.Value) == Convert.ToInt32(Session["UsuarioID"]))
                {
                    item.Selected = true;
                    UsuarioID = Convert.ToInt32(item.Value);
                    break;
                }
            }

            ViewBag.Usuarios = Usuarios;

            ViewModel.TarefasMes = new List<int>();

            for (int i = 1; i <= 12; i++)
            {
                ViewModel.TarefasMes.Add(GraficosDAO.QuantidadeTarefasMes(i, !string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : 0, UsuarioID));
            }

            return View(ViewModel);
        }

        /// <summary>
        /// POST: Gráfico Tarefas
        /// </summary>
        /// <param name="FiltroSetor">ID do setor que será filtrado</param>
        /// <param name="FiltroUsuario">ID do usuário que será filtrado</param>
        /// <returns>Retorna a view com novos dados no model conforme filtros passados</returns>
        [HttpPost, IsLogged]
        public ActionResult Tarefas(int FiltroSetor = 0, int FiltroUsuario = 0)
        {
            var ViewModel = new ViewModelGraficoTarefas();

            ViewModel.TarefasMes = new List<int>();

            for (int i = 1; i <= 12; i++)
            {
                ViewModel.TarefasMes.Add(GraficosDAO.QuantidadeTarefasMes(i, !string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor, FiltroUsuario));
            }

            //Recarrega o dropdownlist de setores setando o valor que havia sido usado como filtro
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

            //Recarrega o dropdownlist de usuários setando o valor que havia sido usado como filtro
            var Usuarios = SelectListMVC.CriaListaSelecao(GraficosDAO.Usuarios(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : FiltroSetor));

            foreach (var item in Usuarios)
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

        /// <summary>
        /// GET: Gráfico Competências
        /// </summary>
        /// <returns>Retorna a view de gráfico das competências</returns>
        [HttpGet, IsLogged]
        public ActionResult Competencias()
        {
            var ViewModel = new ViewModelGraficoCompetencias();

            int usuarioID = 0;

            //Busca todos os usuários e retorna um dictionary contendo os dados e retorna o select list            
            var Usuarios = SelectListMVC.CriaListaSelecao(GraficosDAO.Usuarios(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : 0));

            foreach (var item in Usuarios)
            {
                if (Convert.ToInt32(item.Value) == Convert.ToInt32(Session["UsuarioID"]))
                {
                    item.Selected = true;
                    usuarioID = Convert.ToInt32(item.Value);
                    break;
                }
            }

            ViewBag.Usuarios = Usuarios;

            ViewModel.ListCompetencias = GraficosDAO.Listar();

            if (usuarioID != 0)
            {
                foreach (var item in ViewModel.ListCompetencias)
                {
                    item.Pontos = GraficosDAO.CompetenciaPontos(item.ID, usuarioID);
                }
            }
            else
            {
                foreach (var item in ViewModel.ListCompetencias)
                {
                    item.Pontos = 0;
                }
            }

            return View(ViewModel);
        }

        /// <summary>
        /// POST: Gráfico Competências
        /// </summary>        
        /// <param name="FiltroUsuario">ID do usuário que será filtrado</param>
        /// <returns>Retorna a view com novos dados no model conforme filtros passados</returns>
        [HttpPost, IsLogged]
        public ActionResult Competencias(int FiltroUsuario = 0)
        {
            var ViewModel = new ViewModelGraficoCompetencias();

            ViewModel.ListCompetencias = GraficosDAO.Listar();

            if (FiltroUsuario != 0)
            {
                foreach (var item in ViewModel.ListCompetencias)
                {
                    item.Pontos = GraficosDAO.CompetenciaPontos(item.ID, FiltroUsuario);
                }
            }
            else
            {
                foreach (var item in ViewModel.ListCompetencias)
                {
                    item.Pontos = 0;
                }
            }

            //Recarrega o dropdownlist de usuários setando o valor que havia sido usado como filtro
            var Usuarios = SelectListMVC.CriaListaSelecao(GraficosDAO.Usuarios(!string.IsNullOrEmpty(Convert.ToString(Session["SetorID"])) ? Convert.ToInt32(Session["SetorID"]) : 0));

            foreach (var item in Usuarios)
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