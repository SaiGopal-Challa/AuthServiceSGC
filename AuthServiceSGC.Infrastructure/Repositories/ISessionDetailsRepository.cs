using AuthServiceSGC.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Infrastructure.Repositories
{
    public interface ISessionDetailsRepository
    {
        public Task SaveSessionAndOTPJsonAsync(SessionAndOTPModel sessionAndOtp);

        public Task<SessionAndOTPModel> GetSessionAndOTPFromJsonAsync(string username);

        public Task RemoveSessionAndOTPFromJsonAsync(SessionsDetail sessionsDetails);
    }
}
