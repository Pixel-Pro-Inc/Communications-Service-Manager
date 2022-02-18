using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class UserDto
    {
        public string OrganizationName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string APIKey { get; set; }
    }
}
