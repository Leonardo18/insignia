using Insignia.Model.Tarefa;
using System.Collections.Generic;

namespace Insignia.Painel.ViewModels
{
    public class ViewModelTarefa
    {
        /// <summary>
        /// Lista de tarefas com status fazer
        /// </summary>
        public List<Tarefa> ListFazer { get; set; }

        /// <summary>
        /// Lista de tarefas com status andamento
        /// </summary>
        public List<Tarefa> ListAndamento { get; set; }

        /// <summary>
        /// Lista de tarefas com status finalizadas
        /// </summary>
        public List<Tarefa> ListFinalizadas { get; set; }

        /// <summary>
        /// Lista de tarefas com status em que usuário é participante
        /// </summary>
        public List<Tarefa> ListParticipante { get; set; }
    }
}