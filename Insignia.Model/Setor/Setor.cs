using System.ComponentModel.DataAnnotations;

namespace Insignia.Model.Setor
{
    public class Setor
    {
        /// <summary>
        /// ID do setor no banco de dados
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ID da empresa do cadastro de setor
        /// </summary>
        public int EmpresaID { get; set; }

        /// <summary>
        /// Nome do setor
        /// </summary>
        [Required(ErrorMessage = "Informe o nome do setor")]
        public string Nome { get; set; }
    }
}
