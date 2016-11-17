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

        public string Resumo { get; set; }

        [Required(ErrorMessage = "Informe a descrição da tarefa")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Informe a data de término da tarefa")]
        //[RegularExpression(@"^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]|(?:Jan|Mar|May|Jul|Aug|Oct|Dec)))\1|(?:(?:29|30)(\/|-|\.)(?:0?[1,3-9]|1[0-2]|(?:Jan|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec))\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)(?:0?2|(?:Feb))\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9]|(?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep))|(?:1[0-2]|(?:Oct|Nov|Dec)))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$", ErrorMessage = "A data informada não é valida")]
        public DateTime? Termino { get; set; } = null;

        public string Observacoes { get; set; }

    }
}
