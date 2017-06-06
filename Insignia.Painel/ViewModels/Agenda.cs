using Insignia.Model.Agenda;
using System.Collections.Generic;

namespace Insignia.Painel.ViewModels
{
    public class ViewModelAgenda
    {
        /// <summary>
        /// Lista de eventos da agenda
        /// </summary>
        public List<Agenda> ListAgenda { get; set; }

        /// <summary>
        /// Cor do ícone de sincronizado com Google Calendar
        /// </summary>
        public string IconeRefreshCor { get; set; }
    }
}