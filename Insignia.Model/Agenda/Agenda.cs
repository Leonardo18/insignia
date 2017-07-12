using System;

namespace Insignia.Model.Agenda
{
    public class Agenda
    {
        /// <summary>
        /// Titulo da tarefa na agenda
        /// </summary>
        public string Titulo { get; set; }

        /// <summary>
        /// Descrição da tarefa na agenda
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Data de início na agenda
        /// </summary>
        public DateTime DataInicio { get; set; }

        /// <summary>
        /// Data de Fim na agenda
        /// </summary>
        public DateTime DataFim { get; set; }

        /// <summary>
        /// Cor da tarefa na Agenda
        /// </summary>
        public string Cor { get; set; }
    }
}
