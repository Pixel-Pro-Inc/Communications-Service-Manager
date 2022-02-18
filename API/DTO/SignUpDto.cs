using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class SignUpDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime Dateofbirth { get; set; }
        public string Email { get; set; }
        public int Phonenumber { get; set; }
        public string Password { get; set; }
        public string AccountType { get; set; }
    }
}
