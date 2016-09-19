using System.ComponentModel.DataAnnotations;

namespace Insignia.Model.Empresa
{
    public class Empresa
    {
        [Required(ErrorMessage = "Informe a sua razão social")]
        public string RazaoSocial { get; set; }

        [Required(ErrorMessage = "Informe seu CNPJ")]
        [RegularExpression(@"([0-9]{2}[\.]?[0-9]{3}[\.]?[0-9]{3}[\/]?[0-9]{4}[-]?[0-9]{2})|([0-9]{3}[\.]?[0-9]{3}[\.]?[0-9]{3}[-]?[0-9]{2})", ErrorMessage = "Digite um cnpj válido")]
        public string CNPJ { get; set; }

        [Required(ErrorMessage = "Informe o e-mail")]
        [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Digite um e-mail válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe sua senha")]
        public string SenhaCadastro { get; set; }

        [Required(ErrorMessage = "As senhas não conferem"), Compare("SenhaCadastro")]
        public string ConfirmaSenha { get; set; }


    }
}
