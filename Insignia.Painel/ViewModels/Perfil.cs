using Insignia.Model.Competencia;
using Insignia.Model.Empresa;
using Insignia.Model.Tarefa;
using Insignia.Model.Usuario;
using System.Collections.Generic;

namespace Insignia.Painel.ViewModels
{
    public class ViewModelPerfil
    {
        public List<Tarefa> ListFinalizadas { get; set; }

        public List<Competencia> ListCompetencias { get; set; }

        public List<int> TarefasMes { get; set; }

        public Usuario Usuario { get; set; }

        public Empresa Empresa { get; set; }
    }
}