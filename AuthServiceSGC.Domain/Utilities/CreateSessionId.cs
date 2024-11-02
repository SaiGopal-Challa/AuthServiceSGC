using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Domain.Utilities
{
    public class CreateSessionId
    {
        private static readonly string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "SessionIdSequence.json");

        public static int GetNewSessionId()
        {
            int sessionId;

            // Ensure the file exists
            if (!File.Exists(FilePath))
            {
                // Initialize with 1 if the file does not exist
                sessionId = 1;
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(new { SessionId = sessionId }));
            }
            else
            {
                // Read and deserialize the JSON file
                var json = File.ReadAllText(FilePath);
                var data = JsonConvert.DeserializeObject<SessionData>(json);
                sessionId = data?.SessionId ?? 1; // Fallback to 1 if SessionId is null
            }

            // Increment the session ID
            sessionId++;

            // Update the JSON file with the new session ID
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(new { SessionId = sessionId }));

            return sessionId;
        }

        private class SessionData
        {
            public int SessionId { get; set; }
        }
    }

    public class ConvertClientIDToLoginType
    {
        public async Task<int> GetLoginType(string ClientID, string Username = null)
        {
            int LoginType = 1;
            //return LoginType for ClientId, username from db
            return LoginType;
        }
    }
}
