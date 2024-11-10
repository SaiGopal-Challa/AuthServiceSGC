using AuthServiceSGC.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginUserAsync(LoginDTO loginDto);  // Takes DTO and returns a DTO

        public Task<LogoutResponseDTO> LogoutUserAsync(LogoutRequestDTO logoutRequestDTO);
    }
}
