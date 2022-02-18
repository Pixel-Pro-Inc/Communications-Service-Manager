using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public int PhoneNumber { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string AccountType { get; set; }
        public bool Disabled { get; set; }
        public ICollection<Message> MyProperty { get; set; }

        public string GetUserName()
        {
            return FirstName + ' ' + LastName;
        }
    }
}