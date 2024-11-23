using AuthServiceSGC.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Application.Interfaces
{
    public interface IUserService
    {
        Task<RegisterResultDto> RegisterUserAsync(UserRegisterDto userRegisterDto);

        public Task<string> UpdateAccountAsync(UpdateAccountDto updateAccountDto);

        public Task<string> DeleteAccountAsync(UpdateAccountDto updateAccountDto);
    }
}
