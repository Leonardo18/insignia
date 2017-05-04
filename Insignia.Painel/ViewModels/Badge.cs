using Insignia.Model.Badge;
using System.Collections.Generic;

namespace Insignia.Painel.ViewModels
{
    public class ViewModelBadge
    {
        public Badge Badge { get; set; }

        public List<Badge> ListBadgeBasicas { get; set; }

        public List<Badge> ListBadgeIntermediarias { get; set; }

        public List<Badge> ListBadgeAvancadas { get; set; }        
    }
}