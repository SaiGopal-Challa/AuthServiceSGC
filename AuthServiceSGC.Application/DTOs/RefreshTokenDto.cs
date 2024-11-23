using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Application.DTOs
{
    public class RefreshTokenDto
    {
        public string OldToken { get; set; }
        public string NewToken { get; set; }
    }
}
