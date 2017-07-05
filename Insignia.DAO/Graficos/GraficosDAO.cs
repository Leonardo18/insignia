using Dapper;
using Insignia.Model.Badge;
using Insignia.Model.Competencia;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Insignia.DAO.Graficos
{
    public class GraficosDAO
    {
        private string conStr;

        public GraficosDAO(string conStr)
        {
            this.conStr = conStr;
        }

        /// <summary>
        /// Carrega uma lista com todas as badges encontradas no banco de dados por nível
        /// </summary>
        /// <param name="nivel">Nível qeu está sendo filtrado</param>        
        /// <param name="filtroSetor">Filtro de setor</param>        
        /// <returns>Retorna uma list com bagdes</returns>
        public List<Badge> Badges(string nivel, int filtroSetor)
        {
            List<Badge> list = null;

            using (var sql = new SqlConnection(conStr))
            {
                list = sql.Query<Badge>(" SELECT ID, Titulo, Descricao, Cor, CorFonte, Nivel, Tags, Quantidade FROM Badges WHERE EmpresaID = @EmpresaID AND Nivel = @Nivel AND SetorID = ISNULL(@SetorID, SetorID) ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                        Nivel = nivel,
                        SetorID = Convert.ToString(filtroSetor) != "0" ? Convert.ToString(filtroSetor) : null,                        
                    }).ToList();
            }

            return list;
        }

        /// <summary>
        /// Consulta que busca todas badges adquiridas conforme filtros passados, caso não passe filtros busca todas referente a empresa
        /// </summary>
        /// <param name="filtroSetor">Filtro de setor</param>
        /// <param name="filtroUsuario">Filtro de usuário</param>
        /// <returns>Retorna o número de badges adquiridas</returns>
        public int BadgeAdquiridas(int filtroSetor, int filtroUsuario, int badgeID)
        {
            int resp = 0;

            using (var sql = new SqlConnection(conStr))
            {
                resp = sql.Query<int>(" SELECT Count(Badges.ID) AS Adquiridas FROM Badges WHERE EmpresaID = @EmpresaID AND ID = @BadgeID AND SetorID = ISNULL(@SetorID, SetorID) AND Badges.ID IN (SELECT BadgeID FROM BadgesAdquiridas WHERE UsuarioID = ISNULL(@UsuarioID, UsuarioID)) ",
                     new
                     {
                         EmpresaID = HttpContext.Current.Session["EmpresaID"],
                         BadgeID = badgeID,
                         SetorID = Convert.ToString(filtroSetor) != "0" ? Convert.ToString(filtroSetor) : null,
                         UsuarioID = Convert.ToString(filtroUsuario) != "0" ? Convert.ToString(filtroUsuario) : null
                     }).FirstOrDefault();
            }

            return resp;
        }

        /// <summary>
        /// Busca o total de usuários de um setor, caso não venha filtro de setor pega o total da empresa
        /// </summary>
        /// <param name="filtroSetor">Filtro de setor</param>
        /// <returns>Retorna o número total de usuários</returns>
        public int TotalUsuarios(int filtroSetor)
        {
            int resp = 0;

            using (var sql = new SqlConnection(conStr))
            {
                resp = sql.Query<int>(" SELECT Count(ID) AS TotalUsuarios FROM Usuarios WHERE EmpresaID = @EmpresaID AND SetorID = ISNULL(@SetorID, SetorID)",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                        SetorID = Convert.ToString(filtroSetor) != "0" ? Convert.ToString(filtroSetor) : null
                    }).SingleOrDefault();
            }

            return resp;
        }       

        /// <summary>
        /// Busca a quantidade de tarefas finalizadas em um mês especifico
        /// </summary>
        /// <param name="mes">Mes que será consultado</param>
        /// <returns>Retorna a quantidade de tarefas finalizadas em um mês especifico, caso não ache retorna 0</returns>
        public int QuantidadeTarefasMes(int mes, int filtroSetor, int filtroUsuario)
        {
            int Quantidade = 0;

            if (!string.IsNullOrEmpty(Convert.ToString(mes)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    Quantidade = sql.ExecuteScalar<int>(" SELECT Count(ID) FROM Tarefas WHERE Status = @Status AND Month(CriadoEm) = @Mes AND Year(CriadoEm) = Year(GetDate()) AND EmpresaID = @EmpresaID AND UsuarioID IN (SELECt ID FROM Usuarios WHERE SetorID = ISNULL(@SetorID, SetorID) AND ID = ISNULL(@UsuarioID, ID)) ",
                        new
                        {
                            EmpresaID = HttpContext.Current.Session["EmpresaID"],
                            SetorID = Convert.ToString(filtroSetor) != "0" ? Convert.ToString(filtroSetor) : null,
                            UsuarioID = Convert.ToString(filtroUsuario) != "0" ? Convert.ToString(filtroUsuario) : null,
                            Mes = mes,
                            Status = ConfigurationManager.AppSettings["Finalizada"]
                        });
                }
            }

            return Quantidade;
        }
               
        /// <summary>
        /// Carrega uma lista com todas as competências encontrados no banco de dados.
        /// </summary>
        /// <returns>Retornar uma List de competências</returns>
        public List<Competencia> Listar()
        {
            List<Competencia> list = null;

            using (var sql = new SqlConnection(conStr))
            {
                list = sql.Query<Competencia>(" SELECT ID, EmpresaID, Nome, Descricao FROM Competencias WHERE EmpresaID = @EmpresaID ORDER BY Nome ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"]
                    }).ToList();
            }

            return list;
        }

        /// <summary>
        /// Busca pontos de um usuário em uma competência específica
        /// </summary>
        /// <param name="id">ID da competência</param>
        /// <returns>Retorna 0 caso não tenha pontos ou retorna o número de pontos distribuídos na competência</returns>
        public int CompetenciaPontos(int id, int filtroUsuario)
        {
            int Pontos = 0;

            using (var sql = new SqlConnection(conStr))
            {
                Pontos = sql.ExecuteScalar<int>(" SELECT Pontos FROM CompetenciasUsuarios WHERE EmpresaID = @EmpresaID AND UsuarioID = ISNULL(@UsuarioID, UsuarioID) AND CompetenciaID = @CompetenciaID ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                        UsuarioID = Convert.ToString(filtroUsuario) != "0" ? Convert.ToString(filtroUsuario) : null,
                        CompetenciaID = id,
                    });
            }

            return Pontos;
        }

        /// <summary>
        /// Carrega todos os setores
        /// </summary>
        /// <returns>Dictionary contendo ID e nome de cada setor</returns>
        public Dictionary<int, string> Setores()
        {
            Dictionary<int, string> dict = null;

            using (var sql = new SqlConnection(conStr))
            {
                dict = sql.Query(" SELECT ID, Nome FROM Setores WHERE EmpresaID = @EmpresaID AND ID = ISNULL(@ID, ID) ORDER BY Nome ASC ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                        ID = !string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["SetorID"])) ? HttpContext.Current.Session["SetorID"] : null
                    }).ToDictionary(row => (int)row.ID, row => (string)row.Nome);
            }

            return dict;
        }

        /// <summary>
        /// Carrega todos os usuário de uma empresa e/ou setor do banco
        /// </summary>
        /// <returns>Dictionary contendo o id e o nome dos usuários</returns>
        public Dictionary<int, string> Usuarios(int filtroSetor)
        {
            Dictionary<int, string> dict = null;

            using (var sql = new SqlConnection(conStr))
            {
                dict = sql.Query(" SELECT ID, Nome FROM Usuarios WHERE EmpresaID = @EmpresaID AND SetorID = ISNULL(@SetorID, SetorID) ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                        SetorID = Convert.ToString(filtroSetor) != "0" ? Convert.ToString(filtroSetor) : null
                    }).ToDictionary(row => (int)row.ID, row => (string)row.Nome);
            }

            return dict;
        }
    }
}
