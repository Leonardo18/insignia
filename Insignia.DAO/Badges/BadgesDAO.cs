using Dapper;
using Insignia.DAO.Util;
using Insignia.Model.Badge;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static System.Convert;

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
        /// Recupera as informações de uma badge no banco de dados
        /// </summary>
        /// <param name="id">ID da badge desejada</param>
        /// <returns>Retorna model com as informações da badge</returns>
        public Badge Carregar(int id)
        {
            Badge resp = null;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    resp = sql.Query<Badge>(" SELECT ID, EmpresaID, SetorID, Titulo, Descricao, Cor, CorFonte, Nivel, Tags, Quantidade FROM Badges WHERE ID = @ID AND EmpresaID = @EmpresaID ",
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
        /// Cria uma nova badge no banco de dados
        /// </summary>
        /// <param name="bagde">Badge contendo os dados a serem salvos</param>
        /// <returns>True se o registro foi criado com sucesso, false caso contrário</returns>
        public bool Salvar(Badge badge)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(badge, out resultadoValidacao))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Execute(" INSERT INTO Badges(EmpresaID, SetorID, Titulo, Descricao, Cor, CorFonte, Nivel, Tags, Quantidade) VALUES (@EmpresaID, @SetorID, @Titulo, @Descricao, @Cor, @CorFonte, @Nivel, @Tags, @Quantidade) ",
                                    new
                                    {
                                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                        SetorID = badge.SetorID,
                                        Titulo = badge.Titulo,
                                        Descricao = badge.Descricao,
                                        Cor = badge.Cor,
                                        CorFonte = badge.CorFonte,
                                        Nivel = badge.Nivel,
                                        Tags = badge.Tags,
                                        Quantidade = badge.Quantidade,
                                    });

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Edita uma badge no banco de dados
        /// </summary>
        /// <param name="bagde">Badge contendo a badge a ser editada</param>
        /// <returns>True se o registro foi editado com sucesso, false caso contrário</returns>
        public bool Editar(Badge badge)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(badge, out resultadoValidacao) && !String.IsNullOrEmpty(Convert.ToString(badge.ID)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    var queryResultado = sql.Execute(@" UPDATE Badges SET SetorID = @SetorID, Titulo = @Titulo, Descricao = @Descricao, Cor = @Cor, CorFonte = @CorFonte, Nivel = @Nivel, Tags = @Tags, Quantidade = @Quantidade WHERE ID = @ID ",
                                    new
                                    {
                                        ID = badge.ID,
                                        SetorID = badge.SetorID,
                                        Titulo = badge.Titulo,
                                        Descricao = badge.Descricao,
                                        Cor = badge.Cor,
                                        CorFonte = badge.CorFonte,
                                        Nivel = badge.Nivel,
                                        Tags = badge.Tags,
                                        Quantidade = badge.Quantidade,
                                    });
                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Carrega uma lista com todas as badges encontradas no banco de dados por nível
        /// </summary>
        /// <param name="nivel">Nível qeu está sendo filtrado</param>        
        /// <returns>Retorna uma list com bagdes</returns>
        public List<Badge> Listar(string nivel)
        {
            List<Badge> list = null;

            using (var sql = new SqlConnection(conStr))
            {
                list = sql.Query<Badge>(" SELECT ID, Titulo, Descricao, Cor, CorFonte, Nivel, Tags, Quantidade FROM Badges WHERE EmpresaID = @EmpresaID AND Nivel = @Nivel AND SetorID = ISNULL(@SetorID, SetorID) ",
                   new
                   {
                       EmpresaID = HttpContext.Current.Session["EmpresaID"],
                       Nivel = nivel,
                       SetorID = !string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["SetorID"])) ? HttpContext.Current.Session["SetorID"] : null
                   }).ToList();
            }

            return list;
        }

        /// <summary>
        /// Verifica se existe tarefas para a bagde cadastrada
        /// </summary>
        /// <param name="id">Id da badge.</param>
        /// <returns>Caso exista tarefas para a badge retorna true, se não false</returns>
        public bool PodeRemover(int id)
        {
            bool resp = false;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Query<int>(" SELECT Top 1 ID FROM Tarefas WHERE BadgeID = @BadgeID AND EmpresaID = @EmpresaID",
                        new
                        {
                            BadgeID = id,
                            EmpresaID = HttpContext.Current.Session["EmpresaID"]
                        }).SingleOrDefault();

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Remove uma badge do banco de dados.
        /// </summary>
        /// <param name="id">ID da badge a ser removida</param>
        /// <returns>True se a badge foi encontrada e removida, false caso contrário</returns>
        public bool Remover(int id)
        {
            bool resp = false;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Execute(" DELETE FROM Badges WHERE ID = @ID AND EmpresaID = @EmpresaID ",
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

        /// <summary>
        /// Lista as badges conquistadas de um usuário
        /// </summary>
        /// <param name="UsuarioID">ID do usuário</param>
        /// <param name="nivel">Nível da badge a buscar</param>
        /// <returns>Retornar uma List de Badges adquiridas por um usuário</returns>
        public List<Badge> ListarAdquiridas(int usuarioID, string nivel)
        {
            List<Badge> list;

            using (var sql = new SqlConnection(conStr))
            {
                list = sql.Query<Badge>(" SELECT Badges.ID AS ID, Titulo, Descricao, Cor, CorFonte, Nivel, Tags, Quantidade FROM Badges INNER JOIN BadgesAdquiridas ON Badges.ID = BadgesAdquiridas.BadgeID WHERE Badges.EmpresaID = @EmpresaID AND Badges.Nivel = @Nivel AND BadgesAdquiridas.UsuarioID = @UsuarioID ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                        Nivel = nivel,
                        UsuarioID = usuarioID
                    }).ToList();
            }

            return list;
        }

        /// <summary>
        /// Verifica se o usuário possui a badge em questão
        /// </summary>
        /// <param name="id">ID da badge</param>
        /// <param name="usuarioID">ID do usuário</param>
        /// <returns>Caso tenha adquirido a badge, retorna true se não retorna false</returns>
        public bool VerificaBadgeAdquirida(int id, int usuarioID)
        {
            bool resp = false;

            using (var sql = new SqlConnection(conStr))
            {
                int queryResultado = sql.ExecuteScalar<int>(" SELECT ID FROM BadgesAdquiridas WHERE EmpresaID = @EmpresaID AND BadgeID = @BadgeID AND UsuarioID = @UsuarioID ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                        BadgeID = id,
                        UsuarioID = usuarioID
                    });

                resp = ToBoolean(queryResultado);
            }

            return resp;
        }
    }
}
