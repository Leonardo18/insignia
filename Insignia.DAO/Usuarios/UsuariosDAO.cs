﻿using Dapper;
using Insignia.DAO.Util;
using Insignia.Model.Usuario;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Insignia.DAO.Usuarios
{
    public class UsuariosDAO
    {
        private string conStr;

        public UsuariosDAO(string conStr)
        {
            this.conStr = conStr;
        }

        /// <summary>
        /// Recupera as informações de um usuário no banco de dados
        /// </summary>
        /// <param name="id">ID do usuário desejado</param>
        /// <returns>Retorna model com as informações do usuário</returns>
        public Usuario Carregar(int id)
        {
            Usuario resp = null;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    resp = sql.Query<Usuario>(" SELECT ID, EmpresaID, SetorID, Nome, Email, Tipo FROM Usuarios WHERE ID = @ID AND EmpresaID = @EmpresaID AND SetorID = @SetorID ", new { ID = id, EmpresaID = HttpContext.Current.Session["EmpresaID"], SetorID = HttpContext.Current.Session["SetorID"] }).SingleOrDefault();
                }
            }

            return resp;
        }

        /// <summary>
        /// Cria um novo usuário no banco de dados
        /// </summary>
        /// <param name="usuario">Usuário contendo os dados a serem salvos</param>
        /// <returns>True se o registro foi criado com sucesso, false caso contrário</returns>
        public bool Salvar(Usuario usuario)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(usuario, out resultadoValidacao))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.ExecuteScalar<int>(" INSERT INTO Usuarios(EmpresaID, SetorID, Nome, Email, Tipo) OUTPUT INSERTED.ID VALUES (@EmpresaID, @SetorID, @Nome, @Email, @Tipo) ",
                                    new
                                    {
                                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                        SetorID = HttpContext.Current.Session["SetorID"],
                                        Nome = usuario.Nome,
                                        Email = usuario.Email,
                                        Tipo = usuario.Tipo
                                    });

                    usuario.ID = (int)queryResultado;
                    resp = Convert.ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Edita um usuário no banco de dados
        /// </summary>
        /// <param name="usuario">Model contendo o usuário a ser editado</param>
        /// <returns>True se o usuario foi encontrado e editado, false caso contrário</returns>
        public bool Editar(Usuario usuario)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(usuario, out resultadoValidacao) && !string.IsNullOrEmpty(Convert.ToString(usuario.ID)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    var queryResultado = sql.Execute(" UPDATE Usuarios SET EmpresaID = @EmpresaID, SetorID = @SetorID, Nome = @Nome, Email = @Email WHERE ID = @ID AND Empresa = @EmpresaID AND SetorID = @SetorID",
                                    new
                                    {
                                        ID = usuario.ID,
                                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                        SetorID = HttpContext.Current.Session["SetorID"],
                                        Nome = usuario.Nome,
                                        Email = usuario.Email,
                                        Tipo = usuario.Tipo
                                    });

                    resp = Convert.ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Carrega uma lista com todos os usuários encontrados no banco de dados.
        /// </summary>
        /// <returns>Retornar uma List de usuários</returns>
        public List<Usuario> Listar()
        {
            List<Usuario> list;

            using (var sql = new SqlConnection(conStr))
            {
                list = sql.Query<Usuario>(" SELECT ID, EmpresaID, SetorID, Nome, Email, Tipo FROM Usuarios ORDER BY Nome ").ToList();
            }
            return list;
        }

        /// <summary>
        /// Remove um usuário do banco de dados
        /// </summary>
        /// <param name="id">ID do usuário a ser removido</param>
        /// <returns>True se o usuário foi encontrado e removido, false caso contrário</returns>
        public bool Remover(int id)
        {
            bool resp = false;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Execute(" DELETE FROM Usuarios WHERE ID = @ID AND EmpresaID = @EmpresaID AND SetorID = @SetorID ", new { ID = id, EmpresaID = HttpContext.Current.Session["EmpresaID"], SetorID = HttpContext.Current.Session["SetorID"] });

                    resp = Convert.ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Carrega todos os setores
        /// </summary>
        /// <returns>Dictionary contendo ID e nome de cada setor</returns>
        public Dictionary<int, string> Setores()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();

            using (var sql = new SqlConnection(conStr))
            {
                dict = sql.Query(" SELECT ID, Nome FROM Setores WHERE EmpresaID = @EmpresaID ORDER BY Nome ASC ", new { EmpresaID = HttpContext.Current.Session["EmpresaID"] }).ToDictionary(row => (int)row.ID, row => (string)row.Nome);
            }

            return dict;
        }
    }
}
