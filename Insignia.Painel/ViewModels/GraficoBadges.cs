using Insignia.Model.Badge;
using System.Collections.Generic;

namespace Insignia.Painel.ViewModels
{
    public class ViewModelGraficoBadges
    {
        public int FiltroSetor { get; set; }

        public int FiltroUsuario { get; set; }

        public List<Badge> Badges { get; set; }

        public Dictionary<int, double> PorcentagemBadges { get; set; }

        public int TotalUsuarios { get; set; }

        public int TotalSetores { get; set; }

        public int TotalBadgesAdquiridas { get; set; }

        public int TotalBadgesNaoAdquiridas { get; set; }
    }
}