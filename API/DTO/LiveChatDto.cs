using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTO
{
    public class LiveChatDto
    {
        public string conversationName { get; set; }
        public string participants { get; set; }
        public string apiKey{ get; set; }
    }
}
