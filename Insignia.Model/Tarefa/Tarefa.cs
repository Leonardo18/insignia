using System.ComponentModel.DataAnnotations;

namespace Insignia.Model.Tarefa
{
    public class Tarefa
    {
        public int ID { get; set; }

        public int EmpresaID { get; set; }

        public int UsuarioID { get; set; }

        [Required(ErrorMessage = "Informe o título da tarefa")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Informe a descrição da tarefa")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Informe o tipo da tarefa")]
        public string Tipo { get; set; }

    }
}
