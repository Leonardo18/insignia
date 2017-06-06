using Insignia.Model.Badge;
using System.Collections.Generic;

namespace Insignia.Painel.ViewModels
{
    public class ViewModelGraficoBadges
    {
        /// <summary>
        /// Lista com as badges nível básica
        /// </summary>
        public List<Badge> ListBadgeBasicas { get; set; }

        /// <summary>
        /// Lista com as badges nível intermediária
        /// </summary>
        public List<Badge> ListBadgeIntermediarias { get; set; }

        /// <summary>
        /// Lista com as badges nível avançadas
        /// </summary>
        public List<Badge> ListBadgeAvancadas { get; set; }

        /// <summary>
        /// Total de usuários da empresa
        /// </summary>
        public int TotalUsuarios { get; set; }

        /// <summary>
        /// Total de badge adquiridas
        /// </summary>
        public int TotalBadgesAdquiridas { get; set; }
    }
}