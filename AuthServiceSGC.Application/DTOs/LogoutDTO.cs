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
        public string SessionID { get; set; }
        public string Token { get; set; }
    }
}
