using Insignia.Model.Competencia;
using System.Collections.Generic;

namespace Insignia.Painel.ViewModels
{
    public class ViewModelCompetencia
    {
        public List<Competencia> ListCompetencias { get; set; }

        public int SaldoPontos { get; set; }
    }
}