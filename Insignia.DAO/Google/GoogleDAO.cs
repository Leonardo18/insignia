using Dapper;
using Google.Apis.Json;
using Google.Apis.Util.Store;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Insignia.DAO.Google
{
    public class GoogleDAO : IDataStore
    {
        private string conStr;

        public GoogleDAO(string conStr)
        {
            this.conStr = conStr;

            using (var sql = new SqlConnection(conStr))
            {
                var hold = sql.Query<string>(" SELECT 1 FROM UsuariosGoogle WHERE 1 = 0 ").SingleOrDefault();
            }
        }

        /// <summary>
        /// Stores the given value for the given key. It creates a new file (named <see cref="GenerateStoredKey"/>) in 
        /// <see cref="FolderPath"/>.
        /// </summary>
        /// <typeparam name="T">The type to store in the data store</typeparam>
        /// <param name="key">The key</param>
        /// <param name="value">The value to store in the data store</param>
        public Task StoreAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Chave precisa ter um valor");
            }
            var serialized = NewtonsoftJsonSerializer.Instance.Serialize(value);

            using (var sql = new SqlConnection(conStr))
            {
                string hold = sql.Query<string>(" SELECT Usuario FROM UsuariosGoogle WHERE UsuarioID = @UsuarioID ",
                    new
                    {
                        UsuarioID = key
                    }).SingleOrDefault();

                if (hold == null)
                {
                    int queryResultado = sql.Execute(" INSERT INTO UsuariosGoogle(UsuarioID, TokenAtualizado, Usuario) VALUES (@key, @value, '1') ",
                                    new
                                    {
                                        key = key,
                                        value = serialized.Replace("?LoginGoogle=True", "")
                                    });
                }
                else
                {
                    var queryResultado = sql.Execute(@" UPDATE UsuariosGoogle SET TokenAtualizado = @value WHERE UsuarioID = @key  ",
                                   new
                                   {
                                       key = key,
                                       value = serialized
                                   });
                }
            }

            return Task.Delay(0);
        }

        /// <summary>
        /// Deletes the given key. It deletes the <see cref="GenerateStoredKey"/> named file in <see cref="FolderPath"/>.
        /// </summary>
        /// <param name="key">The key to delete from the data store</param>
        public Task DeleteAsync<T>(string key)
        {

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Chave precisa ter um valor");
            }

            using (var sql = new SqlConnection(conStr))
            {
                int queryResultado = sql.Execute(" DELETE FROM UsuariosGoogle WHERE UsuarioID = @key ",
                    new
                    {
                        key = key
                    });
            }

            return Task.Delay(0);
        }

        /// <summary>
        /// Returns the stored value for the given key or <c>null</c> if the matching file (<see cref="GenerateStoredKey"/>
        /// in <see cref="FolderPath"/> doesn't exist.
        /// </summary>
        /// <typeparam name="T">The type to retrieve</typeparam>
        /// <param name="key">The key to retrieve from the data store</param>
        /// <returns>The stored object</returns>
        public Task<T> GetAsync<T>(string key)
        {
            //Key is the user string sent with AuthorizeAsync
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Chave precisa ter um valor");
            }
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();

            using (var sql = new SqlConnection(conStr))
            {
                string RefreshToken = sql.Query<string>(" SELECT TokenAtualizado FROM UsuariosGoogle WHERE UsuarioID = @UsuarioID; ",
                    new
                    {
                        UsuarioID = key
                    }).SingleOrDefault();

                if (RefreshToken == null)
                {
                    // we don't have a record so we request it of the user.
                    tcs.SetResult(default(T));
                }
                else
                {

                    try
                    {
                        // we have it we use that.
                        tcs.SetResult(NewtonsoftJsonSerializer.Instance.Deserialize<T>(RefreshToken));
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }

                }
            }

            return tcs.Task;
        }

        /// <summary>
        /// Clears all values in the data store. This method deletes all files in <see cref="FolderPath"/>.
        /// </summary>
        public Task ClearAsync()
        {

            using (var sql = new SqlConnection(conStr))
            {
                int queryResultado = sql.Execute(" TRUNCATE TABLE UsuariosGoogle ");
            }

            return Task.Delay(0);
        }

        /// <summary>Creates a unique stored key based on the key and the class type.</summary>
        /// <param name="key">The object key</param>
        /// <param name="t">The type to store or retrieve</param>
        public static string GenerateStoredKey(string key, Type t)
        {
            return string.Format("{0}-{1}", t.FullName, key);
        }
    }
}
