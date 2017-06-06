using Insignia.Model.Agenda;
using Insignia.Model.Competencia;
using Insignia.Model.Tarefa;
using Insignia.Model.Usuario;
using System.Collections.Generic;

namespace Insignia.Painel.ViewModels
{
    public class ViewModelDashboardFuncionario
    {
        /// <summary>
        /// Lista de eventos da agenda
        /// </summary>
        public List<Agenda> ListAgenda { get; set; }

        /// <summary>
        /// Cor do ícone de sincronizado com Google Calendar
        /// </summary>
        public string IconeRefreshCor { get; set; }

        /// <summary>
        /// Lista de tarefas com status fazer
        /// </summary>
        public List<Tarefa> ListFazer { get; set; }

        /// <summary>
        /// Objeto contendo dados de usuário
        /// </summary>
        public Usuario Usuario { get; set; }

        /// <summary>
        /// Lista de competências
        /// </summary>
        public List<Competencia> ListCompetencias { get; set; }
    }
}