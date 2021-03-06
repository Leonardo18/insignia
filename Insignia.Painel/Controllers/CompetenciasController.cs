﻿using Insignia.DAO.Competencias;
using Insignia.DAO.Util;
using Insignia.Model.Competencia;
using Insignia.Painel.Helpers.CustomAttributes;
using Insignia.Painel.ViewModels;
using System;
using System.Configuration;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class CompetenciasController : Controller
    {
        private CompetenciasDAO CompetenciasDAO = new CompetenciasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);


        /// <summary>
        /// GET: Competência Listar
        /// </summary>
        /// <returns>Retorna a view de listar competências com os dados</returns>
        [HttpGet, IsLogged, HavePermission(AreaNome = "Competencias")]
        public ActionResult Listar()
        {
            var CompetenciaModel = CompetenciasDAO.Listar();

            return View(CompetenciaModel);
        }

        /// <summary>
        /// GET: Competencias Adicionar
        /// </summary>
        /// <returns>Retorna a view de adicionar competência</returns>
        [HttpGet, IsLogged, HavePermission(AreaNome = "Competencias")]
        public ActionResult Adicionar()
        {
            var CompetenciaModel = new Competencia();

            return View(CompetenciaModel);
        }

        /// <summary>
        /// POST: Competência Adicionar 
        /// </summary>
        /// <param name="CompetenciaModel">Objeto Model da competência contendo os dados inseridos para cadastro</param>
        /// <returns>Caso consiga validar e salvar a competência faz redirecionamento, se não retorna a view com mensagem</returns>
        [HttpPost, IsLogged, HavePermission(AreaNome = "Competencias")]
        public ActionResult Adicionar(Competencia CompetenciaModel)
        {
            if (ModelState.IsValid)
            {
                if (CompetenciasDAO.Salvar(CompetenciaModel))
                {
                    return RedirectToAction("Editar", new { ID = CompetenciaModel.ID });
                }
            }

            return View(CompetenciaModel);
        }

        /// <summary>
        /// GET: Competência Editar 
        /// </summary>
        /// <param name="ID">ID da competência a ser editada</param>
        /// <returns>Retorna a view com os dados da competência a serem editados</returns>
        [HttpGet, IsLogged, HavePermission(AreaNome = "Competencias")]
        public ActionResult Editar(int ID)
        {
            Competencia CompetenciaModel = CompetenciasDAO.Carregar(ID);

            return View("Editar", CompetenciaModel);
        }

        /// <summary>
        /// POST: Competência Editar
        /// </summary>
        /// <param name="CompetenciaModel">Model contendo os dados da Competencia</param>
        /// <returns>Caso consiga validar os dados e atualizar a competência faz redirecionamento, caso contrário retorna a view novamente para ajuste de dados inválidos</returns>
        [HttpPost, IsLogged, HavePermission(AreaNome = "Competencias")]
        public ActionResult Editar(Competencia CompetenciaModel)
        {
            if (ModelState.IsValid)
            {
                if (CompetenciasDAO.Editar(CompetenciaModel))
                {
                    return RedirectToAction("Editar", new { ID = CompetenciaModel.ID });
                }
            }

            return View("Editar", CompetenciaModel);
        }

        /// <summary>
        /// GET: Competência Remover
        /// </summary>
        /// <param name="ID">ID da competência a ser removida</param>
        /// <returns>Retorna a view com dados da competência que será removida</returns>
        [HttpGet, IsLogged, HavePermission(AreaNome = "Competencias")]
        public ActionResult Remover(int ID)
        {
            //Faz Load com o ID passado
            Competencia CompetenciaModel = CompetenciasDAO.Carregar(ID);

            return View(CompetenciaModel);
        }

        /// <summary>
        /// POST: Competência Remover
        /// </summary>
        /// <param name="CompetenciaModel">Model contendo os dados da competência</param>
        /// <returns>Caso consiga remover a competência do sistema faz redirecionamento, caso contrário retorna a view com mensagem</returns>
        [HttpPost, IsLogged, HavePermission(AreaNome = "Competencias")]
        public ActionResult Remover(Competencia CompetenciaModel)
        {
            //Faz Load com o ID passado
            CompetenciaModel = CompetenciasDAO.Carregar(CompetenciaModel.ID);

            if (CompetenciaModel != null)
            {
                var CompetenciasUsuarios = CompetenciasDAO.VerificaCompetenciaUsuarios(CompetenciaModel.ID);

                //Se existe pontos distribuídos na competência por usuários, redefine saldo
                if (CompetenciasUsuarios != null && CompetenciasUsuarios.Count > 0)
                {
                    foreach (var item in CompetenciasUsuarios)
                    {
                        CompetenciasDAO.RedefinePontosCompetencia(item.Key, item.Value);
                    }
                }

                if (CompetenciasDAO.Remover(CompetenciaModel.ID))
                {
                    return RedirectToAction("Listar");
                }
                else
                {
                    ViewBag.Error = "Ocorreu um erro ao tentar excluir o resgistro, favor entrar em contato com o administrador do sistema";
                }
            }

            return View(CompetenciaModel);
        }

        /// <summary>
        /// GET: Competência Distribuir Pontos
        /// </summary>        
        /// <returns>Retorna a view com dados para distribuir pontos nas competências</returns>
        [HttpGet, IsLogged]
        public ActionResult DistribuirPontos()
        {
            var ViewModel = new ViewModelCompetencia();

            ViewModel.ListCompetencias = CompetenciasDAO.Listar();

            foreach (var item in ViewModel.ListCompetencias)
            {
                item.Pontos = CompetenciasDAO.CompetenciaPontos(item.ID);
            }

            ViewModel.SaldoPontos = CompetenciasDAO.SaldoAtual(0);

            return View(ViewModel);
        }

        /// <summary>
        /// Ativado por um ajax que adiciona pontos na competência de um usuário
        /// </summary>
        /// <param name="ID">ID da competência</param>
        /// <param name="Pontos">Pontos que a competência irá possuir</param>
        /// <param name="Saldo">Saldo que irá ficar</param>
        /// <returns>Retorna 1 para true caso consiga atualizar dados com sucesso e 0 para false</returns>
        public string AdicionarPontos(int ID, int Pontos, int Saldo)
        {
            string resp = string.Empty;

            //Busca saldo atual do usuário
            int SaldoAtual = CompetenciasDAO.SaldoAtual(0);

            //Verifica se o saldo vindo da view e o do banco são os mesmos
            if (SaldoAtual - Saldo == 1)
            {
                resp = "True";
                CompetenciasDAO.AdicionarPontos(ID, Pontos, Saldo);
            }
            else
            {
                resp = "Saldo incorreto, atualize a página e tente novamente.";
            }

            return resp;
        }

        /// <summary>
        /// Ativado por um ajax que remove pontos na competência de um usuário
        /// </summary>
        /// <param name="ID">ID da competência</param>
        /// <param name="Pontos">Pontos que a competência irá possuir</param>
        /// <param name="Saldo">Saldo que irá ficar</param>
        /// <returns>Retorna 1 para true caso consiga atualizar dados com sucesso e 0 para false</returns>
        public string RemoverPontos(int ID, int Pontos, int Saldo)
        {
            string resp = string.Empty;

            //Busca saldo atual do usuário
            int SaldoAtual = CompetenciasDAO.SaldoAtual(0);

            //Verifica se o saldo vindo da view e o do banco são os mesmos
            if (Saldo - SaldoAtual == 1)
            {
                resp = "True";
                CompetenciasDAO.RemoverPontos(ID, Pontos, Saldo);
            }
            else
            {
                resp = "Saldo incorreto, atualize a página e tente novamente.";
            }

            return resp;
        }
    }
}