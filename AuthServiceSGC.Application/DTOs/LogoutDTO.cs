using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Application.DTOs
{
    internal class LogoutDTO
    {
    }
    public class LogoutRequestDTO
    {
        public int SessionID { get; set; }
        public string Token { get; set; }
    }
    public class LogoutResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
