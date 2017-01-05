using Dapper;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace Insignia.DAO.Util
{
    public class Database
    {
        private static string conStr = ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString;

        /// <summary>
        /// Busca uma informação no banco
        /// </summary>
        /// <param name="Tabela">Tabela na qual será buscada informação</param>
        /// <param name="Condicao">Condição para busca</param>
        /// <param name="CondicaoValor">Valor da condição comparativa</param>
        /// <param name="Valor">Valor que será retornar</param>
        /// <returns>Retorna a string com resultado da consulta, se não encontrar nada retorna vazio</returns>
        public static string DBBuscaInfo(string Tabela, string Condicao, string CondicaoValor, string Valor)
        {
            string resp = string.Empty;

            using (var sql = new SqlConnection(conStr))
            {
                resp = sql.Query<string>(" SELECT " + Valor + " FROM " + Tabela + " WHERE " + Condicao + " = @Campo ", new { @Campo = CondicaoValor }).SingleOrDefault();
            }
            return resp;
        }

        /// <summary>
        /// Gera um código aleatório com o número de caracteres soliticado
        /// </summary>
        /// <param name="chars">Número de caracteres</param>
        /// <returns>Retornar string com o código gerado</returns>
        private static string GenerateCode(int chars)
        {
            string resp = string.Empty;
            Random rnd = new Random();

            for (int i = 0; i < chars; i++)
            {
                resp += rnd.Next(0, 9).ToString();
            }

            return resp;
        }
    }
}
