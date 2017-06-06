using Insignia.Model.Badge;
using System.Collections.Generic;

namespace Insignia.Painel.ViewModels
{
    public class ViewModelBadge
    {
        /// <summary>
        /// Objeto de uma badge
        /// </summary>
        public Badge Badge { get; set; }

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
    }
}