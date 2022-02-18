using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class UserDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int Phonenumber { get; set; }
        public string Token { get; set; }
        public bool Disabled { get; set; }
        public string AccountType { get; set; }
    }
}
