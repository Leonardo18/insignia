using Dapper;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Insignia.DAO.Util
{
    public class Hash
    {
        /// <summary>
        /// Gera o código do Token que será usado
        /// </summary>
        /// <param name="length">Length desejado para o hash</param>
        /// <returns>String com o hash gerado</returns>
        public static string Criar(int length)
        {
            string chars = "AaBbCcDdEeFfGdHhIiJjKkLlMnNnOoPpQqRrSsTtUuVvWwXxYyZz01234567898765432100123456789876543210";
            char[] stringChars = new char[length];
            Random random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            string _resp = new String(stringChars);

            return _resp;
        }

        /// <summary>
        /// Valida o hash gerado se já não existe
        /// </summary>
        /// <param name="campo">Campo a ser validado</param>
        /// <param name="tabela">Tabela a ser buscado</param>
        /// <param name="campo2">Valor que será verificado</param>
        /// <returns>Retorna valor gerado</returns>
        public static string ValidaHash(string campo, string tabela, string campo2, int tamanho)
        {
            string hash;

            int queryResultado = 0;

            do
            {
                hash = Criar(tamanho);

                using (var sql = new SqlConnection(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString))
                {
                    queryResultado = sql.ExecuteScalar<int>(" SELECT " + campo + " FROM " + tabela + " WHERE " + campo2 + " = @Hash ", 
                                new
                                {
                                    Hash = hash
                                });
                }

            } while (Convert.ToBoolean(queryResultado));

            return hash;
        }
    }
}
