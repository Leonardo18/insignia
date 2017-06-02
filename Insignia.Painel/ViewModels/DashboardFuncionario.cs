using Insignia.Model.Agenda;
using Insignia.Model.Competencia;
using Insignia.Model.Tarefa;
using Insignia.Model.Usuario;
using System.Collections.Generic;

namespace Insignia.Painel.ViewModels
{
    public class ViewModelDashboardFuncionario
    {
        public List<Agenda> ListAgenda { get; set; }

        public List<Tarefa> ListFazer { get; set; }

        public Usuario Usuario { get; set; }

        public List<Competencia> ListCompetencias { get; set; }

        public string IconeRefreshCor { get; set; }
    }
}