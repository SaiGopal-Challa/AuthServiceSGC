using AuthServiceSGC.Domain.Entities;
using System.Threading.Tasks;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;

namespace AuthServiceSGC.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDbConnection");
        }

        public async Task AddUserAsync(User user)
        {
            using (var connection = new OracleConnection(_connectionString))
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
