using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class ChatMessageDTO
    {
        public string Text { get; set; }
        public int ClientId { get; set; }
        public int RoomId { get; set; }
    }
}
