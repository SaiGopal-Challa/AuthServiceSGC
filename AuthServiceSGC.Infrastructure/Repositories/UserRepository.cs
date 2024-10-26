using AuthServiceSGC.Domain.Entities;
using System.Threading.Tasks;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Text.Json;

namespace AuthServiceSGC.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString_Oracle;
        private readonly string _connectionString;
        private readonly string _jsonFilePath;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString_Oracle = configuration.GetConnectionString("OracleDbConnection");
            _connectionString = configuration.GetConnectionString("PostgresConnection");
            _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Users.json");
            EnsureJsonFileExists();
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
        public async Task AddUserAsyncJson(User user)
        {
            var users = await GetAllUsersAsyncJson();
            users.Add(user);

            // Serialize the updated list and overwrite the file
            var jsonData = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_jsonFilePath, jsonData);
        }

        private async Task<List<User>> GetAllUsersAsyncJson()
        {
            var jsonData = await File.ReadAllTextAsync(_jsonFilePath);
            return JsonSerializer.Deserialize<List<User>>(jsonData) ?? new List<User>();
        }



        // Fetch user from JSON
        public async Task<User> GetUserFromJsonAsync(string username)
        {
            var users = await GetAllUsersAsyncJson();
            return users.Find(u => u.Username == username);
        }

        // Fetch user from PostgreSQL
        public async Task<User> GetUserFromPostgresAsync(string username)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                string query = "SELECT * FROM UserTable WHERE Username = @Username";
                return await connection.QueryFirstOrDefaultAsync<User>(query, new { Username = username });
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

        // Ensure the Users.json file exists, if not, create it
        private void EnsureJsonFileExists()
        {
            if (!File.Exists(_jsonFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_jsonFilePath));
                File.WriteAllText(_jsonFilePath, "[]"); // Initialize with empty array
            }
        }


        //get Email and PhoneNumber from JSON
        public async Task<(string Email, string PhoneNumber)> GetUserContactFromJsonAsync(string username)
        {
            var user = await GetUserFromJsonAsync(username);
            return (user?.Email, user?.PhoneNumber);
        }

        // get Email and PhoneNumber from PostgreSQL
        public async Task<(string Email, string PhoneNumber)> GetUserContactFromPostgresAsync(string username)
        {
            var user = await GetUserFromPostgresAsync(username);
            return (user?.Email, user?.PhoneNumber);
        }
    }
}
