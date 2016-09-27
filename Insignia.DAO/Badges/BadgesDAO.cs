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
    }
}
