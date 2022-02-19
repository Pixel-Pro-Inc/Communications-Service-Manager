using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class SignUpDto
    {
        public string Organization { get; set; }
        public string SenderId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
