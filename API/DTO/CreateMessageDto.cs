using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class CreateMessageDto
    {
        public string[] recipients { get; set; }
        public string content { get; set; }
        public string subject { get; set; }
        //I imagine the first check box for exclusive will be true and having it included will be false. So the check boxes are only tied to this one variable
        public bool Whatsappswitch { get; set; }
    }
}
