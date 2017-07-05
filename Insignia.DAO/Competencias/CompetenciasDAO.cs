using Dapper;
using Insignia.DAO.Util;
using Insignia.Model.Competencia;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static System.Convert;

namespace Insignia.DAO.Competencias
{
    public class CompetenciasDAO
    {
        private string conStr;

        public CompetenciasDAO(string conStr)
        {
            this.conStr = conStr;
        }

        /// <summary>
        /// Recupera as informações de uma competência no banco de dados
        /// </summary>
        /// <param name="id">ID da competência desejada</param>
        /// <returns>Retorna model com as informações da competência</returns>
        public Competencia Carregar(int id)
        {
            Competencia resp = null;

            if (id != 0)
            {
                using (var sql = new SqlConnection(conStr))
                {
                    resp = sql.Query<Competencia>(" SELECT ID, EmpresaID, Nome, Descricao FROM Competencias WHERE ID = @ID AND EmpresaID = @EmpresaID ",
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
        /// Cria uma nova competência no banco de dados
        /// </summary>
        /// <param name="competencia">Competência contendo os dados a serem salvos</param>
        /// <returns>True se o registro foi criado com sucesso, false caso contrário</returns>
        public bool Salvar(Competencia competencia)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao = null;

            if (Validacao.ValidaModel(competencia, out resultadoValidacao))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.ExecuteScalar<int>(" INSERT INTO Competencias(EmpresaID, Nome, Descricao) OUTPUT INSERTED.ID VALUES (@EmpresaID, @Nome, @Descricao) ",
                                    new
                                    {
                                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                        Nome = competencia.Nome,
                                        Descricao = competencia.Descricao
                                    });

                    competencia.ID = (int)queryResultado;

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Edita uma competência no banco de dados
        /// </summary>
        /// <param name="competencia">Competência contendo os dados a serem editados</param>
        /// <returns>True se a competência foi encontrada e editada, false caso contrário</returns>
        public bool Editar(Competencia competencia)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao = null;

            if (Validacao.ValidaModel(competencia, out resultadoValidacao) && competencia.ID != 0)
            {
                using (var sql = new SqlConnection(conStr))
                {
                    var queryResultado = sql.Execute(" UPDATE Competencias SET Nome = @Nome, Descricao = @Descricao WHERE ID = @ID",
                                    new
                                    {
                                        ID = competencia.ID,
                                        Nome = competencia.Nome,
                                        Descricao = competencia.Descricao
                                    });

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
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
        /// Remove uma competência do banco de dados
        /// </summary>
        /// <param name="id">ID da competência a ser removida</param>
        /// <returns>True se a competência foi encontrada e removida, false caso contrário</returns>
        public bool Remover(int id)
        {
            bool resp = false;

            if (id != 0)
            {
                using (var sql = new SqlConnection(conStr))
                {
                    sql.Execute(" DELETE FROM CompetenciasUsuarios WHERE EmpresaID = @EmpresaID AND CompetenciaID = @CompetenciaID ",
                        new
                        {
                            EmpresaID = HttpContext.Current.Session["EmpresaID"],
                            CompetenciaID = id
                        });

                    int queryResultado = sql.Execute(" DELETE FROM Competencias WHERE ID = @ID AND EmpresaID = @EmpresaID ",
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
        /// Busca pontos de um usuário em uma competência específica
        /// </summary>
        /// <param name="id">ID da competência</param>
        /// <returns>Retorna 0 caso não tenha pontos ou retorna o número de pontos distribuídos na competência</returns>
        public int CompetenciaPontos(int id)
        {
            int Pontos = 0;

            using (var sql = new SqlConnection(conStr))
            {
                Pontos = sql.ExecuteScalar<int>(" SELECT Pontos FROM CompetenciasUsuarios WHERE EmpresaID = @EmpresaID AND UsuarioID = @UsuarioID AND CompetenciaID = @CompetenciaID ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                        UsuarioID = HttpContext.Current.Session["UsuarioID"],
                        CompetenciaID = id,
                    });
            }

            return Pontos;
        }

        /// <summary>
        /// Busca saldo de pontos de um usuário
        /// </summary>
        /// <param name="usuarioID">ID do usuário</param>
        /// <returns>Retorna o saldo do usuário</returns>
        public int SaldoAtual(int usuarioID)
        {
            int saldo = 0;

            usuarioID = usuarioID == 0 ? ToInt32(HttpContext.Current.Session["UsuarioID"]) : usuarioID;

            using (var sql = new SqlConnection(conStr))
            {
                saldo = sql.Query<int>(" SELECT Pontos FROM UsuariosPontos WHERE EmpresaID = @EmpresaID AND UsuarioID = @UsuarioID ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                        UsuarioID = usuarioID,
                    }).SingleOrDefault();

                return saldo;
            }
        }

        /// <summary>
        /// Adiciona pontos em uma competência
        /// </summary>
        /// <param name="id">ID da competência</param>
        /// <param name="pontos">Pontos que serão adicionados</param>
        /// <returns>Não possui retorno</returns>
        public void AdicionarPontos(int id, int pontos, int saldo)
        {
            using (var sql = new SqlConnection(conStr))
            {
                int queryResultado = sql.ExecuteScalar<int>(" UPDATE CompetenciasUsuarios SET Pontos = @PontosAdicionados WHERE EmpresaID = @EmpresaID AND UsuarioID = @UsuarioID AND CompetenciaID = @CompetenciaID IF @@ROWCOUNT=0 INSERT INTO CompetenciasUsuarios(EmpresaID, UsuarioID, CompetenciaID, Pontos) VALUES (@EmpresaID, @UsuarioID, @CompetenciaID, @PontosAdicionados) ",
                                new
                                {
                                    EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                    UsuarioID = HttpContext.Current.Session["UsuarioID"],
                                    CompetenciaID = id,
                                    PontosAdicionados = pontos
                                });

                //Atualiza o saldo do usuário
                AtualizaSaldo(saldo);
            }
        }

        /// <summary>
        /// Atualiza o saldo de pontos de um usuário
        /// </summary>
        /// <param name="saldo">Saldo atual do usuário</param>
        /// <returns>Não possui retorno</returns>
        public void AtualizaSaldo(int saldo)
        {
            using (var sql = new SqlConnection(conStr))
            {
                int queryResultado = sql.ExecuteScalar<int>(" UPDATE UsuariosPontos SET Pontos = @Saldo WHERE EmpresaID = @EmpresaID AND UsuarioID = @UsuarioID ",
                                new
                                {
                                    EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                    UsuarioID = HttpContext.Current.Session["UsuarioID"],
                                    Saldo = saldo
                                });
            }
        }

        /// <summary>
        /// Remove pontos de uma competência
        /// </summary>
        /// <param name="id">ID da competência</param>
        /// <param name="pontos">Pontos que serão removidos</param>   
        /// <returns>Não possui retorno</returns>
        public void RemoverPontos(int id, int pontos, int saldo)
        {
            using (var sql = new SqlConnection(conStr))
            {
                int queryResultado = sql.ExecuteScalar<int>(" UPDATE CompetenciasUsuarios SET Pontos = @PontosRemovidos WHERE EmpresaID = @EmpresaID AND UsuarioID = @UsuarioID AND CompetenciaID = @CompetenciaID IF @@ROWCOUNT=0 INSERT INTO CompetenciasUsuarios(EmpresaID, UsuarioID, CompetenciaID, Pontos) VALUES (@EmpresaID, @UsuarioID, @CompetenciaID, @PontosRemovidos) ",
                                new
                                {
                                    EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                    UsuarioID = HttpContext.Current.Session["UsuarioID"],
                                    CompetenciaID = id,
                                    PontosRemovidos = pontos
                                });

                AtualizaSaldo(saldo);
            }
        }

        /// <summary>
        /// Verifica se algum usuário já distribuíu pontos para uma competência
        /// </summary>
        /// <param name="id">ID da competência</param>
        /// <returns>Retorna uma lista com id's de usuários caso exista registro se não retorna a lista vazia</returns>
        public Dictionary<int, int> VerificaCompetenciaUsuarios(int id)
        {
            Dictionary<int, int> resp = null;

            using (var sql = new SqlConnection(conStr))
            {
                resp = sql.Query(" SELECT UsuarioID, Pontos FROM CompetenciasUsuarios WHERE EmpresaID = @EmpresaID AND CompetenciaID = @CompetenciaID",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                        CompetenciaID = id
                    }).ToDictionary(row => (int)row.UsuarioID, row => (int)row.Pontos);
            }

            return resp;
        }

        /// <summary>
        /// Ao excluir uma competência na qual usuários já distribuíram pontos, redefine o saldo do usuário somando esses pontos já distribuídos para redistribuição
        /// </summary>        
        /// <param name="usuarioID">ID do usuário</param>
        /// <param name="pontosDistribuidos">Pontos já distribuídos</param>
        /// <returns>Não possui retorno</returns>
        public void RedefinePontosCompetencia(int usuarioID, int pontosDistribuidos)
        {
            int saldoAtual = SaldoAtual(usuarioID);

            saldoAtual = saldoAtual + pontosDistribuidos;

            using (var sql = new SqlConnection(conStr))
            {
                int queryResultado = sql.ExecuteScalar<int>(" UPDATE UsuariosPontos SET Pontos = @Saldo WHERE EmpresaID = @EmpresaID AND UsuarioID = @UsuarioID ",
                                new
                                {
                                    EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                    UsuarioID = usuarioID,
                                    Saldo = saldoAtual
                                });
            }
        }
    }
}

