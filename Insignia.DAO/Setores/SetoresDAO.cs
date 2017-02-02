using Dapper;
using Insignia.DAO.Util;
using Insignia.Model.Setor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static System.Convert;

namespace Insignia.DAO.Setores
{
    public class SetoresDAO
    {
        private string conStr;

        public SetoresDAO(string conStr)
        {
            this.conStr = conStr;
        }

        /// <summary>
        /// Recupera as informações de um setor no banco de dados
        /// </summary>
        /// <param name="id">ID do setor desejado</param>
        /// <returns>Retorna model com as informações do setor</returns>
        public Setor Carregar(int id)
        {
            Setor resp = null;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    resp = sql.Query<Setor>(" SELECT ID, EmpresaID, Nome FROM Setores WHERE ID = @ID AND EmpresaID = @EmpresaID ",
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
        /// Cria um novo setor no banco de dados
        /// </summary>
        /// <param name="setor">Setor contendo os dados a serem salvos</param>
        /// <returns>True se o registro foi criado com sucesso, false caso contrário</returns>
        public bool Salvar(Setor setor)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(setor, out resultadoValidacao))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.ExecuteScalar<int>(" INSERT INTO Setores(EmpresaID, Nome) OUTPUT INSERTED.ID VALUES (@EmpresaID, @Nome) ",
                                    new
                                    {
                                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                        Nome = setor.Nome,
                                    });

                    setor.ID = (int)queryResultado;
                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Edita um setor no banco de dados
        /// </summary>
        /// <param name="setor">Setor contendo a empresa a ser editada</param>
        /// <returns>True se o setor foi encontrado e editada, false caso contrário</returns>
        public bool Editar(Setor setor)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(setor, out resultadoValidacao) && !string.IsNullOrEmpty(Convert.ToString(setor.ID)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    var queryResultado = sql.Execute(" UPDATE Setores SET Nome = @Nome WHERE ID = @ID",
                                    new
                                    {
                                        ID = setor.ID,
                                        Nome = setor.Nome
                                    });

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Carrega uma lista com todos os setores encontrados no banco de dados.
        /// </summary>
        /// <returns>Retornar uma List de setores</returns>
        public List<Setor> Listar()
        {
            List<Setor> list;

            using (var sql = new SqlConnection(conStr))
            {
                list = sql.Query<Setor>(" SELECT ID, EmpresaID, Nome FROM Setores WHERE EmpresaID = @EmpresaID ORDER BY Nome ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"]
                    }).ToList();
            }

            return list;
        }

        /// <summary>
        /// Remove um setor do banco de dados
        /// </summary>
        /// <param name="id">ID do setor a ser removido</param>
        /// <returns>True se o setor foi encontrado e removido, false caso contrário</returns>
        public bool Remover(int id)
        {
            bool resp = false;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Execute(" DELETE FROM Setores WHERE ID = @ID AND EmpresaID = @EmpresaID ",
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
