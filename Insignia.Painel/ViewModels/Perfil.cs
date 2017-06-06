using Insignia.Model.Competencia;
using Insignia.Model.Empresa;
using Insignia.Model.Tarefa;
using Insignia.Model.Usuario;
using System.Collections.Generic;

namespace Insignia.Painel.ViewModels
{
    public class ViewModelPerfil
    {
        /// <summary>
        /// Lista de tarefas finalizadas
        /// </summary>
        public List<Tarefa> ListFinalizadas { get; set; }

        /// <summary>
        /// Lista de competências
        /// </summary>
        public List<Competencia> ListCompetencias { get; set; }

        /// <summary>
        /// Lista da quantidade de tarefas por mês
        /// </summary>
        public List<int> TarefasMes { get; set; }

        /// <summary>
        /// Objeto contendo dados de usuário
        /// </summary>
        public Usuario Usuario { get; set; }

        /// <summary>
        /// Objetos contendo dados da empresa
        /// </summary>
        public Empresa Empresa { get; set; }
    }
}