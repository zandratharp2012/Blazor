using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ETCDAL
{
    public class SQLDataAccess : ISQLDataAccess
    {
        private readonly IConfiguration _config;

        public string ConnectionStringName { get; set; } = "DW";

        public SQLDataAccess(IConfiguration config)
        {
            _config = config;
        }

        public async Task<List<T>> LoadData<T, U>(string sql, U parameters)
        {
           string connectionString = _config.GetConnectionString(ConnectionStringName);

            try
            {
                using (IDbConnection connection = new SqlConnection(connectionString))
                {
                    var data = await connection.QueryAsync<T>(sql, parameters);
                    return data.ToList();
                }
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw; // Re-throw the exception to ensure it's not swallowed
            }
        }

        public async Task SaveData<T>(string sql, T parameters)
        {
            string connectionString = _config.GetConnectionString(ConnectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(sql, parameters);
            }
        }


    }
}
