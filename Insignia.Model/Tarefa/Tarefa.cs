using System;
using System.ComponentModel.DataAnnotations;

namespace Insignia.Model.Tarefa
{
    public class Tarefa
    {
        public int ID { get; set; }

        public int EmpresaID { get; set; }

        public int UsuarioID { get; set; }

        [Required(ErrorMessage = "Informe o tipo da tarefa")]
        public string TipoID { get; set; }

        [Required(ErrorMessage = "Informe o título da tarefa")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Informe a descrição da tarefa")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Informe a data de término da tarefa")]
        [RegularExpression(@"^(?=\d)(?:(?!(?:(?:0?[5-9]|1[0-4])(?:\.|-|\/)10(?:\.|-|\/)(?:1582))|(?:(?:0?[3-9]|1[0-3])(?:\.|-|\/)0?9(?:\.|-|\/)(?:1752)))(31(?!(?:\.|-|\/)(?:0?[2469]|11))|30(?!(?:\.|-|\/)0?2)|(?:29(?:(?!(?:\.|-|\/)0?2(?:\.|-|\/))|(?=\D0?2\D(?:(?!000[04]|(?:(?:1[^0-6]|[2468][^048]|[3579][^26])00))(?:(?:(?:\d\d)(?:[02468][048]|[13579][26])(?!\x20BC))|(?:00(?:42|3[0369]|2[147]|1[258]|09)\x20BC))))))|2[0-8]|1\d|0?[1-9])([-.\/])(1[012]|(?:0?[1-9]))\2((?=(?:00(?:4[0-5]|[0-3]?\d)\x20BC)|(?:\d{4}(?:$|(?=\x20\d)\x20)))\d{4}(?:\x20BC)?)(?:$|(?=\x20\d)\x20))?((?:(?:0?[1-9]|1[012])(?::[0-5]\d){0,2}(?:\x20[aApP][mM]))|(?:[01]\d|2[0-3])(?::[0-5]\d){1,2})?$", ErrorMessage = "A data informada não é valida")]
        public DateTime? Termino { get; set; } = null;

        public string Observacoes { get; set; }

    }
}
