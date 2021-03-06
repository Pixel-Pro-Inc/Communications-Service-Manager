using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationSenderCode { get; set; }
        public string APIKey { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string AccountType { get; set; }
        public bool Disabled { get; set; }
        public ICollection<int> MessagesRecieved { get; set; }

    }
}