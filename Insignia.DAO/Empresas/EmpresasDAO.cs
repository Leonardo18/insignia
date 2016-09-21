using Dapper;
using Insignia.DAO.Util;
using Insignia.Model.Empresa;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;

namespace Insignia.DAO.Empresas
{
    public class EmpresasDAO
    {
        private string conStr;

        public EmpresasDAO(string conStr)
        {
            this.conStr = conStr;
        }

        /// <summary>
        /// Recupera as informações de uma empresa no banco de dados.
        /// </summary>
        /// <param name="id">ID da empresa desejada.</param>
        /// <returns>Retorna model com as informações da empresa</returns>
        public Empresa Load(string id)
        {
            Empresa resp = null;

            if (!string.IsNullOrWhiteSpace(id))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    resp = sql.Query<Empresa>(" SELECT ID, RazaoSocial, CNPJ, Email FROM Empresas WHERE ID = @ID ", new { ID = id }).SingleOrDefault();
                }
            }

            return resp;
        }

        /// <summary>
        /// Cria uma nova empresa no banco de dados.
        /// </summary>
        /// <param name="user">Empresa contendo os dados a serem salvos.</param>
        /// <returns>True se o registro foi criado com sucesso, false caso contrário.</returns>
        public bool Save(Empresa empresa)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(empresa, out resultadoValidacao))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    Random random = new Random();

                    int id = random.Next();

                    int queryResultado = sql.Execute(" INSERT INTO Empresas(ID, RazaoSocial, CNPJ, Email, Senha) VALUES (@ID, @RazaoSocial, @CNPJ, @Email, @Senha) ",
                                    new
                                    {
                                        ID = id,
                                        RazaoSocial = empresa.RazaoSocial,
                                        CNPJ = empresa.CNPJ,
                                        Email = empresa.Email,
                                        Senha = Util.Autenticacao.Criptografar(empresa.SenhaCadastro)
                                    });

                    empresa.ID = id;
                    resp = Convert.ToBoolean(queryResultado);
                }
            }
            return resp;
        }

        /// <summary>
        /// Edita uma empresa no banco de dados.
        /// </summary>
        /// <param name="user">Empresa contendo a empresa a ser editada.</param>
        /// <returns>True se a empresa foi encontrada e editada, false caso contrário.</returns>
        public bool Editar(Empresa empresa)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(empresa, out resultadoValidacao) && !string.IsNullOrEmpty(Convert.ToString(empresa.ID)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    var queryResultado = sql.Execute(" UPDATE Empresas SET RazaoSocial = @RazaoSocial, CNPJ = @CNPJ, Email = @Email, Senha = @Senha WHERE ID = @ID ",
                                    new
                                    {
                                        ID = empresa.ID,
                                        RazaoSocial = empresa.RazaoSocial,
                                        CNPJ = empresa.CNPJ,
                                        Email = empresa.Email,
                                        Senha = Util.Autenticacao.Criptografar(empresa.SenhaCadastro)
                                    });

                    resp = Convert.ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Remove uma empresa do banco de dados.
        /// </summary>
        /// <param name="id">ID da empresa a ser removida.</param>
        /// <returns>True se a empresa foi encontrada e removida, false caso contrário.</returns>
        public bool Remove(string id)
        {
            bool resp = false;

            if (!string.IsNullOrWhiteSpace(id))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Execute(" DELETE FROM Empresas WHERE ID = @ID ", new { ID = id });

                    resp = Convert.ToBoolean(queryResultado);
                }
            }
            return resp;
        }
    }
}
