namespace Insignia.DAO.Util
{
    public class Utilitarios
    {
        /// <summary>
        /// Criptografa um valor
        /// </summary>
        /// <param name="valor">Valor que deverá ser criptografado</param>
        /// <returns>Retorna o valor criptografado</returns>
        public string Criptografar(string valor)
        {
            string resp = string.Empty;

            if (!string.IsNullOrEmpty(valor))
            {
                resp = Autenticacao.Criptografar(valor);
            }
            return resp;
        }

        /// <summary>
        /// Descriptografa um valor
        /// </summary>
        /// <param name="valor">Valor que deverá ser Descriptografado</param>
        /// <returns>Retorna o valor Descriptografado</returns>
        public string Descriptografar(string valor)
        {
            string resp = string.Empty;

            if (!string.IsNullOrEmpty(valor))
            {
                resp = Autenticacao.Descriptografar(valor);
            }
            return resp;
        }
    }
}
