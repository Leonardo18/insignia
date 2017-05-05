using Dapper;
using Insignia.Model.Badge;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// Carrega todos os setores
        /// </summary>
        /// <returns>Dictionary contendo ID e nome de cada setor</returns>
        public Dictionary<int, string> Setores()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();

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
        public Dictionary<int, string> Usuarios()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();

            using (var sql = new SqlConnection(conStr))
            {
                dict = sql.Query(" SELECT ID, Nome FROM Usuarios WHERE EmpresaID = @EmpresaID AND SetorID = ISNULL(@SetorID, SetorID) ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                        SetorID = !string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["SetorID"])) ? HttpContext.Current.Session["SetorID"] : null
                    }).ToDictionary(row => (int)row.ID, row => (string)row.Nome);
            }

            return dict;
        }

        /// <summary>
        /// Consulta que busca todas badges conforme filtros passados, caso não passe filtros busca todas referente a empresa
        /// </summary>
        /// <param name="filtroSetor">Filtro de setor</param>
        /// <param name="filtroUsuario">Filtro de usuário</param>
        /// <returns>Retorna uma list de badges</returns>
        public List<Badge> Badge(int filtroSetor, int filtroUsuario)
        {
            List<Badge> resp = null;

            using (var sql = new SqlConnection(conStr))
            {
                resp = sql.Query<Badge>(" SELECT Badges.ID, Badges.Titulo FROM Badges WHERE EmpresaID = @EmpresaID AND SetorID = ISNULL(@SetorID, SetorID) AND Badges.ID IN (SELECT BadgeID FROM BadgesAdquiridas WHERe UsuarioID = ISNULL(@UsuarioID, UsuarioID)) ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                        SetorID = Convert.ToString(filtroSetor) != "0" ? Convert.ToString(filtroSetor) : null,
                        UsuarioID = Convert.ToString(filtroUsuario) != "0" ? Convert.ToString(filtroUsuario) : null
                    }).ToList();
            }

            return resp;
        }
    }
}
