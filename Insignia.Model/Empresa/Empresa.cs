using System.ComponentModel.DataAnnotations;

namespace Insignia.Model.Empresa
{
    public class Empresa
    {
        /// <summary>
        /// ID da empresa no banco de dados
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Razão Social da empresa
        /// </summary>
        [Required(ErrorMessage = "Informe a sua razão social")]
        public string RazaoSocial { get; set; }

        /// <summary>
        /// CNPJ da empresa
        /// </summary>
        [Required(ErrorMessage = "Informe seu CNPJ")]
        [RegularExpression(@"([0-9]{2}[\.]?[0-9]{3}[\.]?[0-9]{3}[\/]?[0-9]{4}[-]?[0-9]{2})|([0-9]{3}[\.]?[0-9]{3}[\.]?[0-9]{3}[-]?[0-9]{2})", ErrorMessage = "Digite um cnpj válido")]
        public string CNPJ { get; set; }

        /// <summary>
        /// E-mail de contato da empresa
        /// </summary>
        [Required(ErrorMessage = "Informe o e-mail")]
        [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Digite um e-mail válido")]
        public string Email { get; set; }

        /// <summary>
        /// Senha de acesso ao sistema
        /// </summary>
        [Required(ErrorMessage = "Informe sua senha")]
        public string SenhaCadastro { get; set; }

        /// <summary>
        /// Confirmação da senha digitada
        /// </summary>
        [Required(ErrorMessage = "As senhas não conferem"), Compare("SenhaCadastro")]
        public string ConfirmaSenha { get; set; }

        /// <summary>
        /// Cidade da empresa
        /// </summary>       
        public string Cidade { get; set; }

        /// <summary>
        /// Estado da empresa
        /// </summary>       
        public string Estado { get; set; }

        /// <summary>
        /// País da empresa
        /// </summary>       
        public string Pais { get; set; }

        /// <summary>
        /// Site da empresa
        /// </summary>       
        public string Site { get; set; }

        /// <summary>
        /// Foto da empresa
        /// </summary>       
        public string Foto { get; set; }

        /// <summary>
        /// Cargo da empresa
        /// </summary>       
        public string Cargo { get; set; }
    }
}
