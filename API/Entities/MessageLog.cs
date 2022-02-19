using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class MessageLog
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string Recipient { get; set; }
        public string Body { get; set; }
        public string DateSent { get; set; }
        public string Channel { get; set; }
    }
}
