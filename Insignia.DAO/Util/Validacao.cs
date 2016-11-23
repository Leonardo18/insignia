using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Insignia.DAO.Util
{
    class Validacao
    {
        /// <summary>
        /// Valida os dados de um model. Retorna mensagens com os erros de validação
        /// </summary>
        /// <param name="model"></param>
        /// <param name="results">Lista que conterá os dados da validação (mensagens de erro, caso existam)</param>
        /// <returns>Retorna true caso o Model esteja validado, false caso tenha inconsistências</returns>
        public static bool ValidaModel(object model, out List<ValidationResult> results)
        {
            bool resp = true;

            results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);

            if (!Validator.TryValidateObject(model, context, results))
                resp = false;

            return resp;
        }

        /// <summary>
        /// Valida os dados de um model
        /// </summary>
        /// <param name="model">Model contendo os dados a serem validados</param>
        /// <returns>Retorna true caso o Model esteja validado, false caso tenha inconsistências</returns>
        public static bool ValidaModel(object model)
        {
            bool resp = true;

            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);

            if (!Validator.TryValidateObject(model, context, results))
                resp = false;

            return resp;
        }
    }
}
