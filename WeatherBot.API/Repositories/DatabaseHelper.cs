using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace WeatherBot.API.Repositories
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryAsync<T>(sql, parameters);
            }
        }

        public async Task<int> ExecuteAsync(string sql, object? parameters = null)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.ExecuteAsync(sql, parameters);
            }
        }
    }
}