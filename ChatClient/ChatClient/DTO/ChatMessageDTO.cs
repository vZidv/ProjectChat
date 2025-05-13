using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.DTO
{
    public class ChatMessageDTO : IRequestData
    {
        public string Text { get; set; }
        public int ClientId { get; set; }
        public int RoomId { get; set; }
        public RequestType RequestType { get; }
    }
}
