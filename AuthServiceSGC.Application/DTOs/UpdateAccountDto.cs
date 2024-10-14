using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Application.DTOs
{
    public class UpdateAccountDto
    {
        public string Uid { get; set; } // User ID

        public string Name { get; set; } // User's New Name

        public string Email { get; set; } // User's New Email
    }

}
