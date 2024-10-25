using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Application.DTOs
{
    public class OTPResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? SessionID { get; set; } = null;
    }
}
