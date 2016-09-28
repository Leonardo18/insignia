using Dapper;
using Insignia.DAO.Util;
using Insignia.Model.Badge;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Insignia.DAO.Badges
{
    public class BadgesDAO
    {
        private string conStr;

        public BadgesDAO(string conStr)
        {
            this.conStr = conStr;
        }

        /// <summary>
        /// Recupera as informações de uma badge no banco de dados.
        /// </summary>
        /// <param name="id">ID da empresa desejada.</param>
        /// <returns>Retorna model com as informações da badge</returns>
        public Badge Load(int id)
        {
            Badge resp = null;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    resp = sql.Query<Badge>(" SELECT ID, EmpresaID, Titulo, Subtitulo, Cor, Nivel, Tags FROM Badges WHERE ID = @ID ", new { ID = id }).SingleOrDefault();
                }
            }

            return resp;
        }

        /// <summary>
        /// Cria uma nova badge no banco de dados.
        /// </summary>
        /// <param name="user">Badge contendo os dados a serem salvos.</param>
        /// <returns>True se o registro foi criado com sucesso, false caso contrário.</returns>
        public bool Save(Badge bagde)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(bagde, out resultadoValidacao))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Execute(" INSERT INTO Badges(EmpresaID, Titulo, Subtitulo, Cor, Nivel, Tags) VALUES (@EmpresaID, @Titulo, @Subtitulo, @Cor, @Nivel, @Tags) ",
                                    new
                                    {
                                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                        Titulo = bagde.Titulo,
                                        Subtitulo = bagde.Subtitulo,
                                        Cor = bagde.Cor,
                                        Nivel = bagde.Nivel,
                                        Tags = bagde.Tags,
                                    });

                    resp = Convert.ToBoolean(queryResultado);
                }
            }
            return resp;
        }

        /// <summary>
        /// Edita uma badge no banco de dados.
        /// </summary>
        /// <param name="user">Badge contendo a badge a ser editada.</param>
        /// <returns></returns>
        public bool Editar(Badge badge)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(badge, out resultadoValidacao) && !String.IsNullOrEmpty(Convert.ToString(badge.ID)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    var queryResultado = sql.Execute(@" UPDATE Badges SET Titulo = @Titulo, Subtitulo = @Subtitulo, Cor = @Cor, Tags = @Tags, Nivel = @Nivel WHERE ID = @ID ",
                                    new
                                    {
                                        ID = badge.ID,
                                        Titulo = badge.Titulo,
                                        Subtitulo = badge.Subtitulo,
                                        Cor = badge.Cor,
                                        Tags = badge.Tags,
                                        Nivel = badge.Nivel,
                                    });

                    resp = Convert.ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Carrega uma lista com todas as badges encontradas no banco de dados.
        /// </summary>
        /// <returns>Retornar uma List de Badges</returns>
        public List<Badge> Listar()
        {
            List<Badge> list;

            using (var sql = new SqlConnection(conStr))
            {
                list = sql.Query<Badge>(" SELECT ID, Titulo, Subtitulo, Cor, Nivel, Tags FROM Badges WHERE EmpresaID = @EmpresaID ", new { EmpresaID = HttpContext.Current.Session["EmpresaID"] }).ToList();
            }
            return list;
        }

        /// <summary>
        /// Remove uma badge do banco de dados.
        /// </summary>
        /// <param name="id">ID da badge a ser removida.</param>
        /// <returns>True se a badge foi encontrada e removida, false caso contrário.</returns>
        public bool Remover(int id)
        {
            bool resp = false;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Execute(" DELETE FROM Badges WHERE ID = @ID ", new { ID = id });

                    resp = Convert.ToBoolean(queryResultado);
                }
            }
            return resp;
        }
    }
}
