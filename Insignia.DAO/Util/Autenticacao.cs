using Dapper;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;

namespace Insignia.DAO.Util
{
    public static class Autenticacao
    {
        const string Senha = "E!09#x*&aTe$";

        /// <summary>
        /// Salva Log recebendo a conexão e a mensagem a ser inserida
        /// </summary>
        /// <param name="conStr">Conexão com banco de dados</param>
        /// <param name="mensagem">Mensagem a ser gravada</param>
        public static void Log(string conStr, string mensagem)
        {
            if (!String.IsNullOrEmpty(conStr) && !String.IsNullOrEmpty(mensagem))
            {
                using (var sql = new SqlConnection(conStr))
                {
                    sql.Query(" INSERT INTO Log(Mensagem) VALUES (@Mensagem) ", new { Mensagem = mensagem }).SingleOrDefault();
                }
            }
        }

        /// <summary>
        /// Encriptografa uma senha
        /// </summary>
        /// <param name="senha">Senha a ser criptografada</param>
        /// <returns>Retorna uma senha encriptografada</returns>
        public static string Criptografar(string senha)
        {
            byte[] Results;

            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();

            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Senha));

            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            byte[] DataToEncrypt = UTF8.GetBytes(senha);

            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();

            }

            return Convert.ToBase64String(Results);
        }

        /// <summary>
        /// Método que decriptografa uma senha
        /// </summary>
        /// <param name="senha">Senha a ser descriptografada</param>
        /// <returns>A senha decriptografada</returns>
        public static string Descriptografar(string senha)
        {
            byte[] Results;

            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();

            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Senha));

            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            int mod4 = senha.Length % 4;
            if (mod4 > 0)
            {
                senha += new string('=', 4 - mod4);
            }

            byte[] DataToDecrypt = Convert.FromBase64String(senha);

            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            return UTF8.GetString(Results);
        }
    }
}

