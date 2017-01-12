using System.ComponentModel.DataAnnotations;

namespace Insignia.Model.Competencia
{
    public class Competencia
    {
        /// <summary>
        /// ID da competência no banco de dados
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ID da empresa do cadastro da competência
        /// </summary>
        public int EmpresaID { get; set; }

        /// <summary>
        /// Nome da competência
        /// </summary>
        [Required(ErrorMessage = "Informe o nome da competência")]
        public string Nome { get; set; }

        /// <summary>
        /// Pontos da competência
        /// </summary>        
        public int Pontos { get; set; }
    }
}
