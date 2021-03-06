﻿using Dapper;
using Insignia.DAO.Badges;
using Insignia.DAO.Util;
using Insignia.Model.Agenda;
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

            if (id != 0)
            {
                using (var sql = new SqlConnection(conStr))
                {
                    resp = sql.Query<Tarefa>("SELECT ID, EmpresaID, UsuarioID, BadgeID AS TipoID, Status, Titulo, Resumo, Descricao, Anexo, Termino, Observacoes, CriadoEm, EventoID FROM Tarefas WHERE ID = @ID AND EmpresaID = @EmpresaID AND (UsuarioID = @UsuarioID OR ID IN (SELECT TarefaID FROM TarefasParticipantes WHERE UsuarioID = @UsuarioID)) ",
                        new
                        {
                            ID = id,
                            EmpresaID = HttpContext.Current.Session["EmpresaID"],
                            UsuarioID = HttpContext.Current.Session["UsuarioID"]
                        }).SingleOrDefault();
                }

                resp.Participantes = BuscarParticipantes(id);
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

            List<ValidationResult> resultadoValidacao = null;

            if (Validacao.ValidaModel(tarefa, out resultadoValidacao))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.ExecuteScalar<int>(" INSERT INTO Tarefas(EmpresaID, UsuarioID, BadgeID, Status, Titulo, Resumo, Descricao, Anexo, Termino, Observacoes, CriadoEm, EventoID) OUTPUT INSERTED.ID VALUES (@EmpresaID, @UsuarioID, @BadgeID, @Status, @Titulo, @Resumo, @Descricao, @Anexo, @Termino, @Observacoes, @CriadoEm, @EventoID) ",
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
                                        CriadoEm = DateTime.Now,
                                        EventoID = tarefa.EventoID
                                    });

                    tarefa.ID = (int)queryResultado;

                    resp = ToBoolean(queryResultado);

                    if (tarefa.Participantes != null && tarefa.Participantes.Count > 0)
                    {
                        SalvarParticipantes(tarefa.ID, tarefa.Participantes);
                    }
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

            List<ValidationResult> resultadoValidacao = null;

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

                    RemoverParticipantes(tarefa.ID);

                    if (tarefa.Participantes != null && tarefa.Participantes.Count > 0)
                    {
                        SalvarParticipantes(tarefa.ID, tarefa.Participantes);
                    }
                }
            }

            return resp;
        }

        /// <summary>
        /// Carrega uma lista com todas as tarefas encontradas no banco de dados por status
        /// </summary>
        /// <param name="status">Filtro por status da tarefa</param>
        /// <returns>Retorna uma list de tarefas</returns>
        public List<Tarefa> Listar(string status)
        {
            List<Tarefa> list = null;

            using (var sql = new SqlConnection(conStr))
            {
                list = sql.Query<Tarefa>(" SELECT Top 5 ID, EmpresaID, UsuarioID, BadgeID AS TipoID, Titulo, Resumo, Descricao, Anexo, Termino, Observacoes, CriadoEm FROM Tarefas WHERE EmpresaID = @EmpresaID AND UsuarioID = @UsuarioID AND Status = @Status ORDER BY ID DESC",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                        UsuarioID = HttpContext.Current.Session["UsuarioID"],
                        Status = status
                    }).ToList();
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

            if (id != 0)
            {
                RemoverParticipantes(id);

                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Execute(" DELETE FROM Tarefas WHERE ID = @ID AND EmpresaID = @EmpresaID AND UsuarioID = @UsuarioID ",
                                    new
                                    {
                                        ID = id,
                                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                        UsuarioID = HttpContext.Current.Session["UsuarioID"]
                                    });

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Carrega uma list de tarefas conforme o número passado no top entre o index e o maxIndex
        /// </summary>
        /// <param name="status">Status da tarefa</param>
        /// <param name="index">Index de início da busca</param>
        /// <param name="maxIndex">Index fim da busca</param>
        /// <returns>Retornar uma List de Tarefas</returns>
        public List<Tarefa> ListarTop(string status, int index, int maxIndex)
        {
            List<Tarefa> list;

            using (var sql = new SqlConnection(conStr))
            {
                list = sql.Query<Tarefa>(" SELECT * FROM (SELECT Row_Number() OVER (ORDER BY ID DESC) AS RowIndex, Tarefas.ID, EmpresaID, UsuarioID, BadgeID AS TipoID, Titulo, Resumo, Descricao, Anexo, Termino, Observacoes, CriadoEm FROM Tarefas WHERE EmpresaID = @EmpresaID AND UsuarioID = @UsuarioID AND Status = @Status) AS Sub WHERE Sub.RowIndex > @Index AND Sub.RowIndex <= @MaxIndex ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                        UsuarioID = HttpContext.Current.Session["UsuarioID"],
                        Status = status,
                        Index = index,
                        MaxIndex = maxIndex
                    }).ToList();
            }

            return list;
        }

        /// <summary>
        /// Carrega todos os tipos sendo eles as tags das bagdes
        /// </summary>
        /// <returns>Dictionary contendo ID e tags de cada badges</returns>
        public Dictionary<int, string> Tipos()
        {
            Dictionary<int, string> dict = null;

            using (var sql = new SqlConnection(conStr))
            {
                dict = sql.Query(" SELECT ID, Tags FROM Badges WHERE EmpresaID = @EmpresaID AND SetorID = ISNULL(@SetorID, SetorID) ORDER BY Tags ASC ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                        SetorID = !string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["SetorID"])) ? HttpContext.Current.Session["SetorID"] : null
                    }).ToDictionary(row => (int)row.ID, row => (string)row.Tags);
            }

            return dict;
        }

        /// <summary>
        /// Carrega todos os usuários da empresa e setor
        /// </summary>
        /// <returns>Dictionary contendo ID e nome de cada usuário</returns>
        public Dictionary<int, string> Participantes()
        {
            Dictionary<int, string> dict = null;

            using (var sql = new SqlConnection(conStr))
            {
                dict = sql.Query(" SELECT ID, Nome FROM Usuarios WHERE EmpresaID = @EmpresaID AND SetorID = ISNULL(@SetorID, SetorID) AND ID <> @UsuarioID ORDER BY Nome ASC ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                        SetorID = !string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["SetorID"])) ? HttpContext.Current.Session["SetorID"] : null,
                        UsuarioID = HttpContext.Current.Session["UsuarioID"]
                    }).ToDictionary(row => (int)row.ID, row => (string)row.Nome);
            }

            return dict;
        }

        /// <summary>
        /// Carrega uma lista com todas tarefas na qual o usuário participa
        /// </summary>
        /// <returns>Retornar uma List de Tarefas</returns>
        public List<Tarefa> ListarParticipante(int index, int maxIndex)
        {
            List<Tarefa> list = null;

            using (var sql = new SqlConnection(conStr))
            {
                list = sql.Query<Tarefa>(" SELECT * FROM (SELECT Row_Number() OVER (order by ID) AS RowIndex, Tarefas.ID, EmpresaID, UsuarioID, BadgeID AS TipoID, Titulo, Resumo, Descricao, Anexo, Termino, Observacoes, CriadoEm FROM Tarefas WHERE Tarefas.ID IN (SELECT TarefaID FROM TarefasParticipantes WHERE EmpresaID = @EmpresaID AND UsuarioID = @UsuarioID)) AS Sub  WHERE Sub.RowIndex > @Index AND Sub.RowIndex <= @MaxIndex ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                        UsuarioID = HttpContext.Current.Session["UsuarioID"],
                        Index = index,
                        MaxIndex = maxIndex
                    }).ToList();
            }

            return list;
        }

        /// <summary>
        /// Salva em uma tabela de relacionamento os participantes da tarefa
        /// </summary>
        /// <param name="tarefaID">ID da tarefa</param>
        /// <param name="participantes">Lista com ID dos participantes da tarefa</param>
        /// <returns>Retorna true caso tenha gravado todos com sucesso, false caso contrário</returns>               
        public bool SalvarParticipantes(int tarefaID, List<dynamic> participantes)
        {
            bool resp = false;

            using (var sql = new SqlConnection(conStr))
            {
                foreach (var item in participantes)
                {
                    int queryResultado = sql.ExecuteScalar<int>(" INSERT INTO TarefasParticipantes(EmpresaID, UsuarioID, TarefaID) OUTPUT INSERTED.ID VALUES (@EmpresaID, @UsuarioID, @TarefaID) ",
                                    new
                                    {
                                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                        UsuarioID = item,
                                        TarefaID = tarefaID
                                    });

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Busca participantes de uma tarefa do banco de dados
        /// </summary>
        /// <param name="id">ID da tarefa no qual os participantes irão ser buscados</param>
        /// <returns>True se os participantes foram encontrados, false caso contrário</returns>
        public List<dynamic> BuscarParticipantes(int tarefaID)
        {
            if (tarefaID != 0)
            {
                using (var sql = new SqlConnection(conStr))
                {
                    var participantes = sql.Query(" SELECT UsuarioID FROM TarefasParticipantes WHERE EmpresaID = @EmpresaID AND TarefaID = @TarefaID ",
                                    new
                                    {
                                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                        TarefaID = tarefaID
                                    }).ToList();

                    return participantes;
                }
            }

            return new List<dynamic>();
        }

        /// <summary>
        /// Remove participantes de uma tarefa do banco de dados
        /// </summary>
        /// <param name="id">ID da tarefa no qual os participantes irão ser removidos</param>
        /// <returns>True se os participantes foram removidos, false caso contrário</returns>
        public bool RemoverParticipantes(int tarefaID)
        {
            bool resp = false;

            if (tarefaID != 0)
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Execute(" DELETE FROM TarefasParticipantes WHERE EmpresaID = @EmpresaID AND TarefaID = @TarefaID ",
                                    new
                                    {
                                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                        TarefaID = tarefaID
                                    });

                    resp = ToBoolean(queryResultado);
                }
            }

            return resp;
        }

        /// <summary>
        /// Recupera as informações de um arquivo no banco de dados
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        /// <returns>Retorna string contendo o nome do arquivo</returns>
        public string BuscaArquivo(int id)
        {
            string resp = string.Empty;

            if (id != 0)
            {
                using (var sql = new SqlConnection(conStr))
                {
                    resp = sql.Query<string>(" SELECT Anexo FROM Tarefas WHERE ID = @ID ",
                        new
                        {
                            ID = id
                        }).SingleOrDefault();
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

            if (id != 0 && !string.IsNullOrWhiteSpace(status))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.Execute(" UPDATE Tarefas SET Status = @status WHERE ID = @ID ",
                                    new
                                    {
                                        ID = id,
                                        Status = status
                                    });

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
        /// <returns>Retorna true caso conquiste uma badge, false caso contrário</returns>
        public bool VerificaBadge(string tipoID, int usuarioID)
        {
            bool resp = false;

            int Quantidade = QuantidadeTarefasTipo(ToInt32(tipoID));

            if (Quantidade > 0)
            {
                BadgesDAO BadgesDAO = new BadgesDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

                var Badges = BadgesDAO.Carregar(ToInt32(tipoID));

                if (Quantidade == Badges.Quantidade)
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

                        if (Convert.ToString(HttpContext.Current.Session["EmpresaID"]) != Convert.ToString(HttpContext.Current.Session["UsuarioID"])) { AtualizaSaldoPontos(Badges.Nivel); }

                        resp = true;
                    }
                }
            }

            return resp;
        }

        /// <summary>
        /// Busca a quantidade de tarefas executadas de um tipo especifico
        /// </summary>
        /// <param name="tipID">Tipo da tarefa a ser pesquisado</param>
        /// <returns>Retorna a quantidade de tarefas de um tipo especifico, caso não ache retorna 0</returns>
        private int QuantidadeTarefasTipo(int tipoID)
        {
            int Quantidade = 0;

            if (tipoID != 0)
            {
                using (var sql = new SqlConnection(conStr))
                {
                    Quantidade = sql.ExecuteScalar<int>(" SELECT Count(ID) FROM Tarefas WHERE BadgeID = @BadgeID AND Status = @Status AND EmpresaID = @EmpresaID AND UsuarioID = @UsuarioID ",
                            new
                            {
                                EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                UsuarioID = HttpContext.Current.Session["UsuarioID"],
                                BadgeID = tipoID,
                                Status = ConfigurationManager.AppSettings["Finalizada"]
                            });
                }
            }

            return Quantidade;
        }

        /// <summary>
        /// Adiciona pontos ao usuário por ter desbloqueado uma badge
        /// </summary>
        /// <param name="nivel">Nível da badge adquirida para definir a quantidade de pontos</param>
        private void AtualizaSaldoPontos(string nivel)
        {
            int PontosGanhos = 0;

            if (!string.IsNullOrEmpty(nivel))
            {
                switch (nivel)
                {
                    case "Basica":
                        PontosGanhos = 3;
                        break;
                    case "Intermediaria":
                        PontosGanhos = 7;
                        break;
                    case "Avancada":
                        PontosGanhos = 15;
                        break;
                }

                using (var sql = new SqlConnection(conStr))
                {
                    int queryResultado = sql.ExecuteScalar<int>(" UPDATE UsuariosPontos SET Pontos = Pontos + @PontosGanhos WHERE EmpresaID = @EmpresaID AND UsuarioID = @UsuarioID IF @@ROWCOUNT=0 INSERT INTO UsuariosPontos(EmpresaID, UsuarioID, Pontos) VALUES(@EmpresaID, @UsuarioID, @PontosGanhos) ",
                                    new
                                    {
                                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                        UsuarioID = HttpContext.Current.Session["UsuarioID"],
                                        PontosGanhos = PontosGanhos
                                    });
                }
            }
        }

        /// <summary>
        /// Busca a quantidade de tarefas finalizadas em um mês especifico
        /// </summary>
        /// <param name="mes">Mes que será consultado</param>
        /// <returns>Retorna a quantidade de tarefas finalizadas em um mês especifico, caso não ache retorna 0</returns>
        public int QuantidadeTarefasMes(int mes)
        {
            int Quantidade = 0;

            if (mes != 0)
            {
                using (var sql = new SqlConnection(conStr))
                {
                    Quantidade = sql.ExecuteScalar<int>(" SELECT Count(ID) FROM Tarefas WHERE Status = @Status AND Month(CriadoEm) = @Mes AND Year(CriadoEm) = Year(GetDate()) AND EmpresaID = @EmpresaID AND UsuarioID = @UsuarioID ",
                            new
                            {
                                EmpresaID = HttpContext.Current.Session["EmpresaID"],
                                UsuarioID = HttpContext.Current.Session["UsuarioID"],
                                Mes = mes,
                                Status = ConfigurationManager.AppSettings["Finalizada"]
                            });
                }
            }

            return Quantidade;
        }

        /// <summary>
        /// Lista as tarefas para o model agenda
        /// </summary>
        /// <returns>Retorna uma lista de agenda contendo as tarefas</returns>
        public List<Agenda> ListarTarefasAgenda()
        {
            List<Agenda> list = null;

            using (var sql = new SqlConnection(conStr))
            {
                list = sql.Query<Agenda>(" SELECT Titulo, Resumo, Termino AS DataInicio, '#2C4257' AS Cor FROM Tarefas WHERE EmpresaID = @EmpresaID AND UsuarioID = @UsuarioID ",
                    new
                    {
                        EmpresaID = HttpContext.Current.Session["EmpresaID"],
                        UsuarioID = HttpContext.Current.Session["UsuarioID"],
                    }).ToList();
            }

            return list;
        }
    }
}
