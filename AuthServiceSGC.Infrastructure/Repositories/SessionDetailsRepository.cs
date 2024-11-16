using AuthServiceSGC.Domain.Entities;
using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServiceSGC.Infrastructure.Repositories
{
    public class SessionDetailsRepository : ISessionDetailsRepository
    {
        private readonly string _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Sessions.json");
        private readonly string _postgresConnectionString;

        public SessionDetailsRepository(IConfiguration configuration)
        {
            _postgresConnectionString = configuration.GetConnectionString("PostgresConnection");
            EnsureJsonFileExists();
        }

        // Save or Update SessionAndOTPModel details in JSON
        public async Task SaveSessionAndOTPJsonAsync(SessionAndOTPModel sessionAndOtp)
        {
            var sessions = await GetAllSessionsFromJsonAsync();
            var existingSession = sessions.FirstOrDefault(s => s.Username == sessionAndOtp.Username);

            if (existingSession != null)
            {
                // Update the existing session details
                existingSession.SessionCount = sessionAndOtp.SessionCount;
                existingSession.Sessions = sessionAndOtp.Sessions;
            }
            else
            {
                // Add as a new session entry if none exists
                sessions.Add(sessionAndOtp);
            }

            var jsonData = JsonConvert.SerializeObject(sessions, Formatting.Indented);
            await File.WriteAllTextAsync(_jsonFilePath, jsonData);
        }

        // Save or Update SessionAndOTPModel details in PostgreSQL
        public async Task SaveSessionAndOTPPgSqlAsync(SessionAndOTPModel sessionAndOtp)
        {
            using (var connection = new NpgsqlConnection(_postgresConnectionString))
            {
                string insertOrUpdateQuery = @"
                    INSERT INTO SessionAndOTPTable (Username, SessionCount, Sessions)
                    VALUES (@Username, @SessionCount, @Sessions)
                    ON CONFLICT (Username) DO UPDATE 
                    SET SessionCount = @SessionCount, 
                        Sessions = @Sessions;";

                var parameters = new
                {
                    Username = sessionAndOtp.Username,
                    SessionCount = sessionAndOtp.SessionCount,
                    Sessions = JsonConvert.SerializeObject(sessionAndOtp.Sessions) // Store as JSON
                };

                await connection.ExecuteAsync(insertOrUpdateQuery, parameters);
            }
        }

        // Retrieve SessionAndOTPModel details from JSON
        public async Task<SessionAndOTPModel> GetSessionAndOTPFromJsonAsync(string username)
        {
            var sessions = await GetAllSessionsFromJsonAsync();
            return sessions.FirstOrDefault(s => s.Username == username);
        }

        // Retrieve SessionAndOTPModel details from PostgreSQL
        public async Task<SessionAndOTPModel> GetSessionAndOTPPgSqlAsync(string username)
        {
            using (var connection = new NpgsqlConnection(_postgresConnectionString))
            {
                string query = @"
                    SELECT Username, SessionCount, Sessions
                    FROM SessionAndOTPTable
                    WHERE Username = @Username";

                var result = await connection.QueryFirstOrDefaultAsync<dynamic>(query, new { Username = username });

                if (result == null)
                    return null;

                var sessions = JsonConvert.DeserializeObject<List<SessionsDetail>>(result.Sessions);
                return new SessionAndOTPModel(result.Username)
                {
                    SessionCount = result.SessionCount,
                    Sessions = sessions
                };
            }
        }

        // Ensure the JSON file exists
        private void EnsureJsonFileExists()
        {
            if (!File.Exists(_jsonFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_jsonFilePath));
                File.WriteAllText(_jsonFilePath, "[]"); // Initialize with empty array
            }
        }

        // Helper to get all sessions from JSON
        private async Task<List<SessionAndOTPModel>> GetAllSessionsFromJsonAsync()
        {
            var jsonData = await File.ReadAllTextAsync(_jsonFilePath);
            return JsonConvert.DeserializeObject<List<SessionAndOTPModel>>(jsonData) ?? new List<SessionAndOTPModel>();
        }


        private async Task RemoveSessionAndOTPFromJsonAsync(SessionsDetail sessionsDetails)
        {

        }
    }
}
