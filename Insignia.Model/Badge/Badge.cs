using System.ComponentModel.DataAnnotations;

namespace Insignia.Model.Badge
{
    public class Badge
    {
        /// <summary>
        /// ID da Badge no banco de dados
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ID da empresa do cadastro da badge
        /// </summary>
        public int EmpresaID { get; set; }

        /// <summary>
        /// Titulo da badge
        /// </summary>
        [Required(ErrorMessage = "Informe o título da badge")]
        public string Titulo { get; set; }

        /// <summary>
        /// Subtitulo da badge
        /// </summary>
        [Required(ErrorMessage = "Informe o subtítulo da badge")]
        public string Subtitulo { get; set; }
        
        /// <summary>
        /// Cor em hexadecimal que a badge terá
        /// </summary>
        public string Cor { get; set; }

        /// <summary>
        /// Define conforme a intensidade da cor se a fonte vai ser preta ou branca
        /// </summary>
        public string CorFonte { get; set; }

        /// <summary>
        /// Nível de dificudlade da badge, sendo eles: básica, intermediária e avançada
        /// </summary>
        [Required(ErrorMessage = "Informe o nível da badge")]
        public string Nivel { get; set; }

        /// <summary>
        /// Tags que definem o tipo da tarefa para desbloquea-lá
        /// </summary>
        [Required(ErrorMessage = "Informe as tags da badge")]
        public string Tags { get; set; }
    }
}
