using Dapper;
using Insignia.Model.Empresa;
using Authe = Insignia.DAO.Util.Autenticacao;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace Insignia.DAO.Autenticacao
{
    public class Auth
    {
        private string conStr;

        public Auth(string conStr)
        {
            this.conStr = conStr;
        }

        /// <summary>
        /// Verifica se a empresa existe e recupera os dados dela.
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="senha"></param>
        /// <returns>Model contendo os dados do usuário ou null.</returns>
        public Empresa LoginEmpresa(string email, string senha)
        {
            Empresa model = null;

            using (var sql = new SqlConnection(conStr))
            {
                model = sql.Query<Empresa>(" SELECT ID, RazaoSocial, CNPJ, Email, Senha As SenhaCadastro FROM Empresas WHERE Email = @Email ", new { Email = email }).SingleOrDefault();
            }

            if (model != null)
            {
                string senhaDB = model.SenhaCadastro;

                if (senhaDB != Authe.Criptografar(senha))
                {
                    model = null;
                }
            }
            return model;
        }


        /// <summary>
        /// Verifica se o usuário atual possui permissões 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="areaID"></param>
        /// <returns>Retorna true se tiver permissão e false para não tem permissão</returns>
        public bool VerificaPermissao(string userID, string areaID)
        {
            int result = 0;


            using (var sql = new SqlConnection(conStr))
            {
                result = sql.ExecuteScalar<int>(" SELECT COUNT(*) FROM UsuariosXAreas WHERE UsuarioID = @UsuarioID AND AreaID = @AreaID", new { UsuarioID = userID, AreaID = areaID });
            }

            return Convert.ToBoolean(result);
        }
    }
}
