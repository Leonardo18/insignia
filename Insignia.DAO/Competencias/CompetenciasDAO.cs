using Dapper;
using Insignia.DAO.Util;
using Insignia.Model.Competencia;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static System.Convert;

namespace Insignia.DAO.Competencias
{
    public class CompetenciasDAO
    {
        private string conStr;

        public CompetenciasDAO(string conStr)
        {
            this.conStr = conStr;
        }

        /// <summary>
        /// Recupera as informações de uma competência no banco de dados
        /// </summary>
        /// <param name="id">ID da competência desejada</param>
        /// <returns>Retorna model com as informações da competência</returns>
        public Competencia Carregar(int id)
        {
            Competencia resp = null;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    resp = sql.Query<Competencia>(" SELECT ID, EmpresaID, Nome FROM Competencias WHERE ID = @ID AND EmpresaID = @EmpresaID ",
                        new
                        {
                            ID = id,
                            EmpresaID = HttpContext.Current.Session["EmpresaID"]
                        }).SingleOrDefault();
                }
            }

            return resp;
        }

        /// <summary>
        /// Cria uma nova competência no banco de dados
        /// </summary>
        /// <param name="competencia">Competência contendo os dados a serem salvos</param>
        /// <returns>True se o registro foi criado com sucesso, false caso contrário</returns>
        public bool Salvar(Competencia competencia)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(competencia, out resultadoValidacao))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.ExecuteScalar<int>(" INSERT INTO Competencias(EmpresaID, Nome) OUTPUT INSERTED.ID VALUES (@EmpresaID, @Nome) ",
                                    new
                                    {
                                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                        Nome = competencia.Nome,
                                    });

                    competencia.ID = (int)queryResultado;
                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Edita uma competência no banco de dados
        /// </summary>
        /// <param name="competencia">Competência contendo a empresa a ser editada</param>
        /// <returns>True se a competência foi encontrada e editada, false caso contrário</returns>
        public bool Editar(Competencia competencia)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(competencia, out resultadoValidacao) && !string.IsNullOrEmpty(Convert.ToString(competencia.ID)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    var queryResultado = sql.Execute(" UPDATE Competencias SET Nome = @Nome WHERE ID = @ID",
                                    new
                                    {
                                        ID = competencia.ID,
                                        Nome = competencia.Nome
                                    });

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Carrega uma lista com todos as competências encontrados no banco de dados.
        /// </summary>
        /// <returns>Retornar uma List de competências</returns>
        public List<Competencia> Listar()
        {
            List<Competencia> list;

            using (var sql = new SqlConnection(conStr))
            {
                list = sql.Query<Competencia>(" SELECT ID, EmpresaID, Nome FROM Competencias WHERE EmpresaID = @EmpresaID ORDER BY Nome ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"]
                    }).ToList();
            }

            return list;
        }

        /// <summary>
        /// Remove uma competência do banco de dados
        /// </summary>
        /// <param name="id">ID da competência a ser removida</param>
        /// <returns>True se a competência foi encontrada e removida, false caso contrário</returns>
        public bool Remover(int id)
        {
            bool resp = false;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Execute(" DELETE FROM Competencias WHERE ID = @ID AND EmpresaID = @EmpresaID ",
                        new
                        {
                            ID = id,
                            EmpresaID = HttpContext.Current.Session["EmpresaID"]
                        });

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }
    }
}
