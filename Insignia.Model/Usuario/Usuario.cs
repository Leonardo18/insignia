using System.ComponentModel.DataAnnotations;

namespace Insignia.Model.Usuario
{
    public class Usuario
    {
        /// <summary>
        /// ID do usuário no banco de dados
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ID da empresa no qual usuário pertence
        /// </summary>
        public int EmpresaID { get; set; }

        /// <summary>
        /// ID do setor no qual o usuário pertence
        /// </summary>
        [Required(ErrorMessage = "Informe o setor do usuário")]
        public int SetorID { get; set; }

        /// <summary>
        /// Token do usuário
        /// </summary>       
        public string Token { get; set; }

        /// <summary>
        /// Nome do Setor
        /// </summary>
        public string SetorNome { get; set; }

        /// <summary>
        /// Nome do usuário
        /// </summary>
        [Required(ErrorMessage = "Informe o nome do usuário")]
        public string Nome { get; set; }

        /// <summary>
        /// Email do usuário
        /// </summary>
        [Required(ErrorMessage = "Informe o e-mail do usuário")]
        public string Email { get; set; }

        /// <summary>
        /// Senha de acesso ao sistema
        /// </summary>        
        public string SenhaCadastro { get; set; }

        /// <summary>
        /// Confirmação da senha digitada
        /// </summary>        
        public string ConfirmaSenha { get; set; }

        /// <summary>
        /// Tipo do usuário sendo eles: Funcionário e Gestor
        /// </summary>
        [Required(ErrorMessage = "Informe o tipo do usuário")]
        public string Tipo { get; set; }

        /// <summary>
        /// Cidade do usuário
        /// </summary>       
        public string Cidade { get; set; }

        /// <summary>
        /// Estado do usuário
        /// </summary>       
        public string Estado { get; set; }

        /// <summary>
        /// País do usuário
        /// </summary>       
        public string Pais { get; set; }

        /// <summary>
        /// Site do usuário
        /// </summary>       
        public string Site { get; set; }

        /// <summary>
        /// Foto do usuário
        /// </summary>       
        public string Foto { get; set; }

        /// <summary>
        /// Cargo do usuário
        /// </summary>       
        public string Cargo { get; set; }
    }
}
