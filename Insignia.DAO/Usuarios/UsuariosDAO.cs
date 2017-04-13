using Dapper;
using Insignia.DAO.Util;
using Insignia.Model.Usuario;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static System.Convert;

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
        /// Recupera as informações de um usuário no banco de dados
        /// </summary>
        /// <param name="id">ID do usuário desejado</param>
        /// <returns>Retorna model com as informações do usuário</returns>
        public Usuario Carregar(int id)
        {
            Usuario resp = null;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    resp = sql.Query<Usuario>(" SELECT ID, EmpresaID, SetorID, Nome, Email, Cidade, Estado, Pais, Site, Foto, Cargo, Tipo FROM Usuarios WHERE ID = @ID AND EmpresaID = @EmpresaID ",
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
        /// Cria um novo usuário no banco de dados
        /// </summary>
        /// <param name="usuario">Usuário contendo os dados a serem salvos</param>
        /// <returns>True se o registro foi criado com sucesso, false caso contrário</returns>
        public bool Salvar(Usuario usuario)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(usuario, out resultadoValidacao))
            {
                usuario.Token = Hash.ValidaHash("ID", "Usuarios", "Token", 50);

                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.ExecuteScalar<int>(" INSERT INTO Usuarios(EmpresaID, SetorID, Nome, Email, Tipo, Token) OUTPUT INSERTED.ID VALUES (@EmpresaID, @SetorID, @Nome, @Email, @Tipo, @Token) ",
                                    new
                                    {
                                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                        SetorID = usuario.SetorID,
                                        Nome = usuario.Nome,
                                        Email = usuario.Email,
                                        Tipo = usuario.Tipo,
                                        Token = usuario.Token,
                                    });

                    usuario.ID = (int)queryResultado;
                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Edita um usuário no banco de dados
        /// </summary>
        /// <param name="usuario">Model contendo o usuário a ser editado</param>
        /// <returns>True se o usuario foi encontrado e editado, false caso contrário</returns>
        public bool Editar(Usuario usuario)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(usuario, out resultadoValidacao) && !string.IsNullOrEmpty(Convert.ToString(usuario.ID)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    var queryResultado = sql.Execute(" UPDATE Usuarios SET EmpresaID = @EmpresaID, SetorID = @SetorID, Nome = @Nome, Email = @Email WHERE ID = @ID AND EmpresaID = @EmpresaID",
                                    new
                                    {
                                        ID = usuario.ID,
                                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                        SetorID = usuario.SetorID,
                                        Nome = usuario.Nome,
                                        Email = usuario.Email,
                                        Tipo = usuario.Tipo
                                    });

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Edita o perfil de um usuário no banco de dados
        /// </summary>
        /// <param name="usuario">Model contendo o usuário a ser editado</param>
        /// <returns>True se o usuario foi encontrado e editado, false caso contrário</returns>
        public bool EditarPerfil(Usuario usuario)
        {
            bool resp = false;

            using (var sql = new SqlConnection(conStr))
            {
                var queryResultado = sql.Execute(" UPDATE Usuarios SET Cidade = @Cidade, Estado = @Estado, Pais = @Pais, Site = @Site, Foto = @Foto, Cargo = @Cargo WHERE ID = @ID AND EmpresaID = @EmpresaID",
                                new
                                {
                                    ID = usuario.ID,
                                    EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                    Cidade = usuario.Cidade,
                                    Estado = usuario.Estado,
                                    Pais = usuario.Pais,
                                    Site = usuario.Site,
                                    Foto = usuario.Foto,
                                    Cargo = usuario.Cargo
                                });

                resp = ToBoolean(queryResultado);
            }

            return resp;
        }

        /// <summary>
        /// Carrega uma lista com todos os usuários encontrados no banco de dados.
        /// </summary>
        /// <returns>Retornar uma List de usuários</returns>
        public List<Usuario> Listar()
        {
            List<Usuario> list;

            using (var sql = new SqlConnection(conStr))
            {
                list = sql.Query<Usuario>(" SELECT ID, EmpresaID, SetorID, Nome, Email, Tipo FROM Usuarios WHERE EmpresaID = @EmpresaID ORDER BY Nome ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"]
                    }).ToList();
            }

            return list;
        }

        /// <summary>
        /// Remove um usuário do banco de dados
        /// </summary>
        /// <param name="id">ID do usuário a ser removido</param>
        /// <returns>True se o usuário foi encontrado e removido, false caso contrário</returns>
        public bool Remover(int id)
        {
            bool resp = false;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    sql.Execute(" DELETE FROM TarefasParticipantes WHERE UsuarioID = @ID AND EmpresaID = @EmpresaID ",
                        new
                        {
                            ID = id,
                            EmpresaID = HttpContext.Current.Session["EmpresaID"]
                        });

                    sql.Execute(" DELETE FROM UsuariosPontos WHERE UsuarioID = @ID AND EmpresaID = @EmpresaID ",
                        new
                        {
                            ID = id,
                            EmpresaID = HttpContext.Current.Session["EmpresaID"]
                        });

                    sql.Execute(" DELETE FROM CompetenciasUsuarios WHERE UsuarioID = @ID AND EmpresaID = @EmpresaID ",
                        new
                        {
                            ID = id,
                            EmpresaID = HttpContext.Current.Session["EmpresaID"]
                        });

                    sql.Execute(" DELETE FROM UsuariosGoogle WHERE UsuarioID = @ID ",
                        new
                        {
                            ID = Convert.ToString(id)
                        });

                    int queryResultado = sql.Execute(" DELETE FROM Usuarios WHERE ID = @ID AND EmpresaID = @EmpresaID ",
                        new
                        {
                            ID = id,
                            EmpresaID = HttpContext.Current.Session["EmpresaID"]
                        });

                    if (ToBoolean(queryResultado))
                    {
                        sql.Execute(" DELETE FROM BadgesAdquiridas WHERE UsuarioID = @UsuarioID AND EmpresaID = @EmpresaID ",
                            new
                            {
                                UsuarioID = id,
                                EmpresaID = HttpContext.Current.Session["EmpresaID"]
                            });

                        sql.Execute(" DELETE FROM Tarefas WHERE UsuarioID = @UsuarioID AND EmpresaID = @EmpresaID ",
                            new
                            {
                                UsuarioID = id,
                                EmpresaID = HttpContext.Current.Session["EmpresaID"]
                            });
                    }

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
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
                dict = sql.Query(" SELECT ID, Nome FROM Setores WHERE EmpresaID = @EmpresaID ORDER BY Nome ASC ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"]
                    }).ToDictionary(row => (int)row.ID, row => (string)row.Nome);
            }

            return dict;
        }

        /// <summary>
        /// Cria senha para um novo usuário no sistema
        /// </summary>
        /// <param name="token">Token de identificação do usuário</param>
        /// <param name="senha">senha criada</param>
        /// <returns>Retorna true caso tenha criado a senha com sucesso, false caso contrário</returns>
        public bool CriarSenha(string token, string senha)
        {
            bool resp = false;

            if (!string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(senha))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Execute(" UPDATE Usuarios SET Senha = @Senha, AtivadoEm = @AtivadoEm WHERE Token = @Token ",
                        new
                        {
                            Token = token,
                            Senha = Util.Autenticacao.Criptografar(senha),
                            AtivadoEm = DateTime.Now
                        });
                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Verifica se o usuário já existe no sistema
        /// </summary>
        /// <param name="email">Email que está sendo cadastrado</param>        
        /// <returns>True se não existe e false caso já exista</returns>
        public bool VerificaUsuario(int id, string email)
        {
            bool resp = true;
            Usuario usuario = null;

            using (var sql = new SqlConnection(conStr))
            {
                usuario = sql.Query<Usuario>(" SELECT ID FROM Usuarios WHERE Email = @Email ",
                    new
                    {
                        Email = email,
                        EmpresaID = HttpContext.Current.Session["EmpresaID"]
                    }).FirstOrDefault();
            }

            if (usuario != null && !string.IsNullOrEmpty(email) && (id != 0 && usuario.ID != id))
            {
                if (!string.IsNullOrEmpty(Convert.ToString(usuario.ID)))
                {
                    resp = false;
                }
            }

            if (usuario == null)
            {
                resp = false;
            }

            return resp;
        }

        /// <summary>
        /// Verifica se o token do usuário já foi ativado
        /// </summary>
        /// <param name="token">Token do usuário</param>
        /// <returns>Retorna true caso não tenha sido ativado, false caso contrário</returns>
        public bool VerificaToken(string token)
        {
            bool resp = false;

            if (!string.IsNullOrWhiteSpace(token))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.ExecuteScalar<int>(" SELECT ID FROM Usuarios WHERE Token = @Token AND NOT AtivadoEm IS NULL ",
                        new
                        {
                            Token = token
                        });
                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Atualiza a senha em um cadastro
        /// </summary>
        /// <param name="email">email na qual será atualizado a senha</param>
        /// <param name="senha">senha nova</param>
        /// <returns>Retorna true caso tenha alterado com sucesso, false caso contrário</returns>
        public bool AtualizaSenha(string email, string senha)
        {
            bool resp = false;

            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(senha))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Execute(" UPDATE Usuarios SET Senha = @Senha WHERE Email = @Email ",
                        new
                        {
                            Email = email,
                            Senha = Util.Autenticacao.Criptografar(senha)
                        });
                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }


        /// <summary>
        /// Gera novo token pois o e-mail de um usuário foi alterado para outro
        /// </summary>
        /// <param name="usuario">Model contendo dados do usuário</param>
        /// <returns>True se o token editado, false caso contrário</returns>
        public bool AtualizaToken(Usuario usuario)
        {
            bool resp = false;

            if (!string.IsNullOrEmpty(Convert.ToString(usuario.ID)))
            {
                usuario.Token = Hash.ValidaHash("ID", "Usuarios", "Token", 50);

                using (var sql = new SqlConnection(conStr))
                {
                    var queryResultado = sql.Execute(" UPDATE Usuarios SET Token = @Token, AtivadoEm = NULL WHERE ID = @ID AND EmpresaID = @EmpresaID",
                                    new
                                    {
                                        ID = usuario.ID,
                                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                        Token = usuario.Token
                                    });

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Carrega todos os estados do banco
        /// </summary>
        /// <returns>Dictionary contendo a Sigla e nome de cada estado</returns>
        public Dictionary<string, string> Estados()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            using (var sql = new SqlConnection(conStr))
            {
                dict = sql.Query(" SELECT Sigla, Nome FROM Estados ORDER BY Nome ASC ").ToDictionary(row => (string)row.Sigla, row => (string)row.Nome);
            }

            return dict;
        }
    }
}
