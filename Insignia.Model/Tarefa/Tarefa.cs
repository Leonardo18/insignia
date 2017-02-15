using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Insignia.Model.Tarefa
{
    public class Tarefa
    {
        /// <summary>
        /// ID da tarefa no banco de dados
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ID da empresa do cadastro da tarefa
        /// </summary>
        public int EmpresaID { get; set; }

        /// <summary>
        /// ID do usuário responável pela tarefa
        /// </summary>
        public int UsuarioID { get; set; }

        /// <summary>
        /// Tipo da tarefa a ser executada, sendo ele definido pelas tags de alguma badge
        /// </summary>
        [Required(ErrorMessage = "Informe o tipo da tarefa")]
        public string TipoID { get; set; }

        /// <summary>
        /// Status da tarefa
        /// </summary>        
        public string Status { get; set; }

        /// <summary>
        /// Titulo da tarefa
        /// </summary>
        [Required(ErrorMessage = "Informe o título da tarefa")]
        public string Titulo { get; set; }

        /// <summary>
        /// Resumo da tarefa
        /// </summary>
        public string Resumo { get; set; }

        /// <summary>
        /// Descrição detalhada do que deve ser feito na tarefa
        /// </summary>
        [Required(ErrorMessage = "Informe a descrição da tarefa")]
        public string Descricao { get; set; }

        /// <summary>
        /// Anexo de algum arquivo referente a tarefa
        /// </summary>
        public string Anexo { get; set; }

        /// <summary>
        /// Usuários participantes
        /// </summary>
        public List<int> Participantes { get; set; }

        /// <summary>
        /// Data para término da tarefa
        /// </summary>
        [Required(ErrorMessage = "Informe a data de término da tarefa")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])[\/\.-](0[13578]|1[02])[\/\.-]((19|[2-9]\d)\d{2})\s(0[0-9]|1[0-9]|2[0-3]):([0-5][0-9]):([0-5][0-9]))|((0[1-9]|[12]\d|30)[\/\.-](0[13456789]|1[012])[\/\.-]((19|[2-9]\d)\d{2})\s(0[0-9]|1[0-9]|2[0-3]):([0-5][0-9]):([0-5][0-9]))|((0[1-9]|1\d|2[0-8])[\/\.-](02)[\/\.-]((19|[2-9]\d)\d{2})\s(0[0-9]|1[0-9]|2[0-3]):([0-5][0-9]):([0-5][0-9]))|((29)[\/\.-](02)[\/\.-]((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))\s(0[0-9]|1[0-9]|2[0-3]):([0-5][0-9]):([0-5][0-9])))$", ErrorMessage = "A data informada não é valida")]
        public DateTime? Termino { get; set; } = null;

        /// <summary>
        /// Observações referente a tarefa
        /// </summary>
        public string Observacoes { get; set; }

        /// <summary>
        /// Observações referente a tarefa
        /// </summary>
        public DateTime? CriadoEm { get; set; }
    }
}
