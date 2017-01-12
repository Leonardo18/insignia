using Insignia.Model.Tarefa;
using System.Collections.Generic;

namespace Insignia.Painel.ViewModels
{
    public class ViewModelTarefa
    {
        public List<Tarefa> ListFazer { get; set; }
        public List<Tarefa> ListAndamento { get; set; }
        public List<Tarefa> ListFinalizadas { get; set; }            
    }
}