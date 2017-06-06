using Insignia.Model.Competencia;
using System.Collections.Generic;

namespace Insignia.Painel.ViewModels
{
    public class ViewModelCompetencia
    {
        /// <summary>
        /// Lista de competências
        /// </summary>
        public List<Competencia> ListCompetencias { get; set; }

        /// <summary>
        /// Saldo de pontos adquiridos
        /// </summary>
        public int SaldoPontos { get; set; }
    }
}