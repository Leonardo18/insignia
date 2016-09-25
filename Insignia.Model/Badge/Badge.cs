using System.ComponentModel.DataAnnotations;

namespace Insignia.Model.Badge
{
    public class Badge
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Informe o título da badge")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Informe o subtítulo da badge")]
        public string Subtitulo { get; set; }

        //[Required(ErrorMessage = "Informe a cor da badge")]
        public string Cor { get; set; }

        [Required(ErrorMessage = "Informe o nível da badge")]
        public string Nivel { get; set; }

        [Required(ErrorMessage = "Informe as tags da badge")]
        public string Tags { get; set; }

    }
}
