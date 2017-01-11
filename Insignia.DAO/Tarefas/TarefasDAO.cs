using Dapper;
using Insignia.DAO.Badges;
using Insignia.DAO.Util;
using Insignia.Model.Tarefa;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static System.Convert;

namespace Insignia.DAO.Tarefas
{
    public class TarefasDAO
    {
        private string conStr;

        public TarefasDAO(string conStr)
        {
            this.conStr = conStr;
        }

        /// <summary>
        /// Recupera as informações de uma tarefa no banco de dados
        /// </summary>
        /// <param name="id">ID da tarefa desejada</param>
        /// <returns>Retorna model com as informações da tarefa</returns>
        public Tarefa Carregar(int id)
        {
            Tarefa resp = null;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    resp = sql.Query<Tarefa>("SELECT ID, EmpresaID, UsuarioID, BadgeID AS TipoID, Status, Titulo, Resumo, Descricao, Anexo, Termino, Observacoes, CriadoEm FROM Tarefas WHERE ID = @ID AND EmpresaID = @EmpresaID AND UsuarioID = @UsuarioID ", new { ID = id, EmpresaID = HttpContext.Current.Session["EmpresaID"], UsuarioID = HttpContext.Current.Session["UsuarioID"] }).SingleOrDefault();
                }
            }

            return resp;
        }

        /// <summary>
        /// Cria uma nova tarefa no banco de dados
        /// </summary>
        /// <param name="tarefa">Tarefa contendo os dados a serem salvos</param>
        /// <returns>True se o registro foi criado com sucesso, false caso contrário</returns>
        public bool Salvar(Tarefa tarefa)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(tarefa, out resultadoValidacao))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.ExecuteScalar<int>(" INSERT INTO Tarefas(EmpresaID, UsuarioID, BadgeID, Status, Titulo, Resumo, Descricao, Anexo, Termino, Observacoes, CriadoEm) OUTPUT INSERTED.ID VALUES (@EmpresaID, @UsuarioID, @BadgeID, @Status, @Titulo, @Resumo, @Descricao, @Anexo, @Termino, @Observacoes, @CriadoEm) ",
                                    new
                                    {
                                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                        UsuarioID = HttpContext.Current.Session["UsuarioID"],
                                        BadgeID = tarefa.TipoID,
                                        Status = tarefa.Status,
                                        Titulo = tarefa.Titulo,
                                        Resumo = tarefa.Resumo,
                                        Descricao = tarefa.Descricao,
                                        Anexo = tarefa.Anexo,
                                        Termino = tarefa.Termino,
                                        Observacoes = tarefa.Observacoes,
                                        CriadoEm = DateTime.Now
                                    });
                    tarefa.ID = (int)queryResultado;

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Edita dados de uma tarefa no banco de dados
        /// </summary>
        /// <param name="tarefa">Tarefa contendo os dados a serem salvos</param>
        /// <returns>Retorna true caso tenha salvo com sucesso, false caso tenha dado erro</returns>
        public bool Editar(Tarefa tarefa)
        {
            bool resp = false;

            List<ValidationResult> resultadoValidacao;

            if (Validacao.ValidaModel(tarefa, out resultadoValidacao) && !string.IsNullOrEmpty(Convert.ToString(tarefa.ID)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    var queryResultado = sql.Execute(@" UPDATE Tarefas SET BadgeID = @BadgeID, Titulo = @Titulo, Resumo = @Resumo, Descricao = @Descricao, Anexo = @Anexo, Termino = @Termino, Observacoes = @Observacoes WHERE ID = @ID ",
                                    new
                                    {
                                        ID = tarefa.ID,
                                        BadgeID = tarefa.TipoID,
                                        Titulo = tarefa.Titulo,
                                        Resumo = tarefa.Resumo,
                                        Descricao = tarefa.Descricao,
                                        Anexo = tarefa.Anexo,
                                        Termino = tarefa.Termino,
                                        Observacoes = tarefa.Observacoes,
                                    });

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Carrega uma lista com todas as tarefas encontradas no banco de dados por status
        /// </summary>
        /// <returns>Retornar uma List de Tarefas</returns>
        public List<Tarefa> Listar(string status)
        {
            List<Tarefa> list;

            using (var sql = new SqlConnection(conStr))
            {
                list = sql.Query<Tarefa>(" SELECT ID, EmpresaID, UsuarioID, BadgeID AS TipoID, Titulo, Resumo, Descricao, Anexo, Termino, Observacoes, CriadoEm FROM Tarefas WHERE EmpresaID = @EmpresaID AND UsuarioID = @UsuarioID AND Status = @Status ", new { EmpresaID = HttpContext.Current.Session["EmpresaID"], UsuarioID = HttpContext.Current.Session["UsuarioID"], Status = status }).ToList();
            }

            return list;
        }

        /// <summary>
        /// Remove uma tarefa do banco de dados
        /// </summary>
        /// <param name="id">ID da badge a ser removida</param>
        /// <returns>True se a tarefa foi encontrada e removida, false caso contrário</returns>
        public bool Remover(int id)
        {
            bool resp = false;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Execute(" DELETE FROM Tarefas WHERE ID = @ID AND EmpresaID = @EmpresaID AND UsuarioID = @UsuarioID ", new { ID = id, EmpresaID = HttpContext.Current.Session["EmpresaID"], UsuarioID = HttpContext.Current.Session["UsuarioID"] });

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Carrega todos os tipos sendo eles as tags das bagdes
        /// </summary>
        /// <returns>Dictionary contendo ID e tags de cada badges</returns>
        public Dictionary<int, string> Tipos()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();

            using (var sql = new SqlConnection(conStr))
            {
                dict = sql.Query(" SELECT ID, Tags FROM Badges WHERE EmpresaID = @EmpresaID ORDER BY Tags ASC ", new { EmpresaID = HttpContext.Current.Session["EmpresaID"] }).ToDictionary(row => (int)row.ID, row => (string)row.Tags);
            }

            return dict;
        }

        /// <summary>
        /// Recupera as informações de um arquivo no banco de dados
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        /// <returns>Retorna string contendo o nome do arquivo</returns>
        public string BuscaArquivo(int id)
        {
            string resp = string.Empty;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    resp = sql.Query<string>(" SELECT Anexo FROM Tarefas WHERE ID = @ID ", new { ID = id }).SingleOrDefault();
                }
            }

            return resp;
        }

        /// <summary>
        /// Atualiza o status de uma tarefa para em andamento ou finalizada
        /// </summary>
        /// <param name="id">ID da tarefa na qual o status será atualizado</param>
        /// <param name="status">Status que será definido pra tarefa</param>
        /// <returns>Caso consiga atulizar com sucesso retorna true, caso contrario retorna false</returns>
        public bool AtualizaStatus(int id, string status)
        {
            bool resp = false;

            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)) && !string.IsNullOrWhiteSpace(status))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Execute(" UPDATE Tarefas SET Status = @status WHERE ID = @ID ", new { ID = id, Status = status });

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Verifica se a tarefa que está sendo finalizada desbloqueia uma badge
        /// </summary>
        /// <param name="TipoID">ID da badge em questão</param>
        /// <param name="UsuarioID">ID do usuário da tarefa</param>
        public void VerificaBadge(string tipoID, int usuarioID)
        {
            int Quantidade = QuantidadeTarefas(ToInt32(tipoID));

            if (Quantidade > 0)
            {
                BadgesDAO BadgesDAO = new BadgesDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

                var Badges = BadgesDAO.Carregar(ToInt32(tipoID));

                if (Quantidade >= Badges.Quantidade)
                {
                    using (var sql = new SqlConnection(conStr))
                    {
                        int queryResultado = sql.ExecuteScalar<int>(" INSERT INTO BadgesAdquiridas(EmpresaID, UsuarioID, BadgeID, ConquistadoEm) OUTPUT INSERTED.ID VALUES (@EmpresaID, @UsuarioID, @BadgeID, @ConquistadoEm) ",
                                        new
                                        {
                                            EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                            UsuarioID = HttpContext.Current.Session["UsuarioID"],
                                            BadgeID = Badges.ID,
                                            ConquistadoEm = DateTime.Now
                                        });
                    }
                }
            }
        }

        /// <summary>
        /// Busca a quantidade de tarefas executadas de um tipo especifico
        /// </summary>
        /// <param name="BadgeID"></param>
        /// <returns>Retorna a quantidade de tarefas de um tipo especifico, caos não ache retorna 0</returns>
        private int QuantidadeTarefas(int tipoID)
        {
            int Quantidade = 0;

            if (!string.IsNullOrEmpty(Convert.ToString(tipoID)))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    Quantidade = sql.ExecuteScalar<int>(" SELECT Count(ID) FROM Tarefas WHERE BadgeID = @BadgeID AND Status = @Status ", new { BadgeID = tipoID, Status = ConfigurationManager.AppSettings["Finalizada"] });
                }
            }

            return Quantidade;
        }
    }
}
