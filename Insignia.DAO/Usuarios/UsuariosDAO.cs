using Dapper;
using Insignia.DAO.Util;
using Insignia.Model.Usuario;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;

namespace Insignia.DAO.Usuarios
{
    public class UsuariosDAO
    {
        private string conStr;

        public UsuariosDAO(string conStr)
        {
            this.conStr = conStr;
        }

        /// <summary>
        /// Recupera as informações de um usuário no banco de dados.
        /// </summary>
        /// <param name="id">ID do usuário desejado.</param>
        /// <returns>Retorna model com as informações do usuário</returns>
        public Usuario Load(string id)
        {
            Usuario resp = null;

            if (!String.IsNullOrWhiteSpace(id))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    //resp = sql.Query<Usuario>(" SELECT ID, Ativo, Nome, Usuario AS Login, Email, CreatedOn, Autor, UpdatedOn, AutorUpdate, TipoID FROM Usuarios WHERE ID = @ID ", new { ID = id }).SingleOrDefault();
                }
            }

            return resp;
        }

        /// <summary>
        /// Remove um usuário do banco de dados.
        /// </summary>
        /// <param name="id">ID do usuário a ser removido.</param>
        /// <returns>True se o usuário foi encontrado e removido, false caso contrário.</returns>
        public bool Remove(string id)
        {
            bool resp = false;

            if (!String.IsNullOrWhiteSpace(id))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Execute(" DELETE FROM Usuarios WHERE ID = @ID ", new { ID = id });

                    resp = Convert.ToBoolean(queryResultado);
                }
            }
            return resp;
        }

        /// <summary>
        /// Cria um novo usuário no banco de dados.
        /// </summary>
        /// <param name="user">Usuario contendo os dados a serem salvos.</param>
        /// <returns>True se o registro foi criado com sucesso, false caso contrário.</returns>
        public bool Save(Usuario user)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(user, out resultadoValidacao))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    Random random = new Random();

                    string id = Convert.ToString(random.Next());

                    //int queryResultado = sql.Execute(@" INSERT INTO Usuarios(ID, Ativo, Nome, Usuario, Senha, CreatedOn, Autor, UpdatedOn, AutorUpdate, TipoID, Email)
                    //                                VALUES(@ID, @Ativo, @Nome, @Usuario, @Senha, @CreatedOn, @Autor, @UpdatedOn, @AutorUpdate, @TipoID, @Email) ",
                    //                new
                    //                {
                    //                    ID = id,
                    //                    Ativo = user.Ativo,
                    //                    Nome = user.Nome,
                    //                    Usuario = user.Login,
                    //                    Senha = Insignia.DAO.Util.Autenticacao.Criptografar(user.Senha),
                    //                    Email = user.Email,
                    //                    TipoID = user.TipoID,
                    //                    CreatedOn = DateTime.Now,
                    //                    Autor = Convert.ToString(HttpContext.Current.Session["UsuarioNome"]),
                    //                    UpdatedOn = DateTime.Now,
                    //                    AutorUpdate = Convert.ToString(HttpContext.Current.Session["UsuarioNome"])
                    //                });

                    //user.ID = id;
                    //resp = Convert.ToBoolean(queryResultado);
                }
            }
            return resp;
        }

        /// <summary>
        /// Edita um usuário no banco de dados.
        /// </summary>
        /// <param name="user">Usuario contendo o usuário a ser editado.</param>
        /// <returns></returns>
        public bool Editar(Usuario user)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(user, out resultadoValidacao) && !String.IsNullOrEmpty("user.ID"))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    //var queryResultado = sql.Execute(@" UPDATE Usuarios SET Ativo = @Ativo, Nome = @Nome, Usuario = @Usuario, Senha = @Senha,
                    //                                Email = @Email, UpdatedOn = @UpdatedOn, AutorUpdate = @AutorUpdate, TipoID = @TipoID WHERE ID = @ID ",
                    //                new
                    //                {
                    //                    ID = user.ID,
                    //                    Ativo = user.Ativo,
                    //                    Nome = user.Nome,
                    //                    Usuario = user.Login,
                    //                    Senha = Insignia.DAO.Util.Autenticacao.Criptografar(user.Senha),
                    //                    Email = user.Email,
                    //                    TipoID = user.TipoID,
                    //                    UpdatedOn = DateTime.Now,
                    //                    AutorUpdate = Convert.ToString(HttpContext.Current.Session["UsuarioNome"])
                    //                });

                    //resp = Convert.ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Carrega uma lista com todos os usuários encontrados no banco de dados.
        /// </summary>
        /// <returns>Retornar uma List de Usuario</returns>
        public List<Usuario> Listar()
        {
            List<Usuario> list;

            using (var sql = new SqlConnection(conStr))
            {
                list = sql.Query<Usuario>(" SELECT ID, Ativo, TipoID, Nome, Usuario AS Login, Email FROM Usuarios ").ToList();
            }
            return list;
        }

        /// <summary>
        /// Recupera count da tabela de Usuarios.
        /// </summary>
        /// <returns></returns>
        public int GetUsuariosCount()
        {
            int resp = 0;

            using (var sql = new SqlConnection(conStr))
            {
                resp = sql.Query(" SELECT COUNT(*) FROM Usuarios ").SingleOrDefault();
            }

            return resp;
        }

        /// <summary>
        /// Carrega todos os tipos de usuário disponíveis.
        /// </summary>
        /// <returns>Dictionary contendo ID e Nome de cada tipo.</returns>
        public Dictionary<string, string> Tipos()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            using (var sql = new SqlConnection(conStr))
            {
                dict = sql.Query(" SELECT ID, Nome FROM UsuariosTipos ORDER BY Nome ASC ").ToDictionary(row => (string)row.ID, row => (string)row.Nome);
            }

            return dict;
        }
    }
}
