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

        public static int GetNewSessionId()
        {
            int SessionId = 1234;
            //return next value from a sequence, 

            return SessionId;
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
