using Dapper;
using Insignia.DAO.Util;
using Insignia.Model.Empresa;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using static System.Convert;

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
        /// Recupera as informações de uma empresa no banco de dados
        /// </summary>
        /// <param name="id">ID da empresa desejada</param>
        /// <returns>Retorna model com as informações da empresa</returns>
        public Empresa Carregar(int id)
        {
            Empresa resp = null;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    resp = sql.Query<Empresa>(" SELECT ID, RazaoSocial, CNPJ, Email, Senha AS SenhaCadastro, Cidade, Estado, Pais, Site, Foto FROM Empresas WHERE ID = @ID ", new { ID = id }).SingleOrDefault();
                }
            }

            return resp;
        }

        /// <summary>
        /// Cria uma nova empresa no banco de dados
        /// </summary>
        /// <param name="empresa">Empresa contendo os dados a serem salvos</param>
        /// <returns>True se o registro foi criado com sucesso, false caso contrário</returns>
        public bool Salvar(Empresa empresa)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(empresa, out resultadoValidacao))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.ExecuteScalar<int>(" INSERT INTO Empresas(RazaoSocial, CNPJ, Email, Senha) OUTPUT INSERTED.ID VALUES (@RazaoSocial, @CNPJ, @Email, @Senha) ",
                                    new
                                    {
                                        RazaoSocial = empresa.RazaoSocial,
                                        CNPJ = empresa.CNPJ,
                                        Email = empresa.Email,
                                        Senha = Util.Autenticacao.Criptografar(empresa.SenhaCadastro)
                                    });

                    empresa.ID = (int)queryResultado;
                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Edita uma empresa no banco de dados
        /// </summary>
        /// <param name="empresa">Empresa contendo a empresa a ser editada</param>
        /// <returns>True se a empresa foi encontrada e editada, false caso contrário</returns>
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

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Remove uma empresa do banco de dados
        /// </summary>
        /// <param name="id">ID da empresa a ser removida</param>
        /// <returns>True se a empresa foi encontrada e removida, false caso contrário</returns>
        public bool Remover(int id)
        {
            bool resp = false;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Execute(" DELETE FROM Empresas WHERE ID = @ID ", new { ID = id });

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Verifica se a empresa já existe no sistema
        /// </summary>
        /// <param name="email">Email que está sendo cadastrado</param>
        /// <param name="cnpj">Cnpj que está sendo cadastrado</param>
        /// <returns>True se não existe e false caso já exista</returns>
        public bool VerificaEmpresa(string email, string cnpj)
        {
            bool resp = true;
            Empresa empresa = null;

            using (var sql = new SqlConnection(conStr))
            {
                empresa = sql.Query<Empresa>(" SELECT ID FROM Empresas WHERE Email = @Email OR CNPJ = @CNPJ ", new { Email = email, CNPJ = cnpj }).FirstOrDefault();
            }

            if (empresa != null && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(cnpj))
            {
                if (!string.IsNullOrEmpty(Convert.ToString(empresa.ID)))
                {
                    resp = false;
                }
            }

            if (!string.IsNullOrEmpty(email) && string.IsNullOrEmpty(cnpj) && empresa == null)
            {
                resp = false;
            }

            return resp;
        }

        /// <summary>
        /// Atualiza a senha em um cadastro
        /// </summary>
        /// <param name="email">email na qual será atualizado a senha</param>
        /// <param name="senha">senha nova</param>
        /// <returns>Retorna true caso tenha alterado com sucesso, false caso contrário</returns>
        public bool AtualizaSenha(string email, string senha)
        {
            bool resp = false;

            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(senha))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Execute(" UPDATE Empresas SET Senha = @Senha WHERE Email = @Email ", new { Email = email, Senha = Util.Autenticacao.Criptografar(senha) });

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }
    }
}
