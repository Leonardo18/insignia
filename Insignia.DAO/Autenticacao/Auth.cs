﻿using Dapper;
using Insignia.Model.Empresa;
using Authe = Insignia.DAO.Util.Autenticacao;
using System.Data.SqlClient;
using System.Linq;
using Insignia.Model.Usuario;

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
        /// Verifica se a empresa existe e recupera os dados dela
        /// </summary>
        /// <param name="email">Email do usuário de acesso</param>
        /// <param name="senha">Senha de acesso do usuário</param>
        /// <returns>Model contendo os dados da empresa ou null</returns>                    
        public Empresa LoginEmpresa(string email, string senha)
        {
            Empresa model = null;

            using (var sql = new SqlConnection(conStr))
            {
                model = sql.Query<Empresa>(" SELECT ID, RazaoSocial, CNPJ, Email, Senha As SenhaCadastro, Foto FROM Empresas WHERE Email = @Email ",
                    new
                    {
                        Email = email
                    }).SingleOrDefault();
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
        /// Verifica se o usuário existe e recupera os dados dele
        /// </summary>
        /// <param name="email">Email do usuário de acesso</param>
        /// <param name="senha">Senha de acesso do usuário</param>
        /// <returns>Model contendo os dados do usuário ou null</returns>
        public Usuario LoginUsuario(string email, string senha)
        {
            Usuario model = null;

            using (var sql = new SqlConnection(conStr))
            {
                model = sql.Query<Usuario>(" SELECT ID, EmpresaID, SetorID, Nome, Email, Senha AS SenhaCadastro, Foto, Tipo FROM Usuarios WHERE Email = @Email ",
                    new
                    {
                        Email = email
                    }).SingleOrDefault();
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
    }
}
