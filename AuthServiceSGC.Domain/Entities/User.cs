using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Domain.Entities
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }

        [DefaultValue(1)]
        public int? LoginType  { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
