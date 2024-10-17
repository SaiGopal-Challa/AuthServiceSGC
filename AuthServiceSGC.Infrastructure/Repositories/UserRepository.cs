using AuthServiceSGC.Domain.Entities;
using System.Threading.Tasks;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace AuthServiceSGC.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString_Oracle;
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString_Oracle = configuration.GetConnectionString("OracleDbConnection");
            _connectionString = configuration.GetConnectionString("PostgresConnection");
        }

        public async Task AddUserAsync(User user)
        {
            // Use NpgsqlConnection instead of OracleConnection
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                // Updated query syntax for PostgreSQL (no ':' prefix)
                string insertQuery = @"INSERT INTO UserTable (Username, Password, Email, PhoneNumber) 
                                       VALUES (@Username, @Password, @Email, @PhoneNumber)";

                var parameters = new DynamicParameters();
                parameters.Add("Username", user.Username);
                parameters.Add("Password", user.Password);
                parameters.Add("Email", user.Email);
                parameters.Add("PhoneNumber", user.PhoneNumber);

                // Execute the query asynchronously
                await connection.ExecuteAsync(insertQuery, parameters);
            }
        }

        public async Task AddUserAsync_Oracle(User user)
        {
            using (var connection = new OracleConnection(_connectionString_Oracle))
            {
                string insertQuery = @"INSERT INTO UserTable (Username, Password, Email, PhoneNumber) 
                                       VALUES (:Username, :Password, :Email, :PhoneNumber)";

                var parameters = new DynamicParameters();
                parameters.Add("Username", user.Username);
                parameters.Add("Password", user.Password);
                parameters.Add("Email", user.Email);
                parameters.Add("PhoneNumber", user.PhoneNumber);

                await connection.ExecuteAsync(insertQuery, parameters);
            }
        }
    }
}
