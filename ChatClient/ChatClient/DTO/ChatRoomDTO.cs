using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.DTO
{
    public class ChatRoomDTO
    {
        public string Name { get; set; } = null!;
        public int OwnerId { get; set; }

        public RequestType RequestType => RequestType.CreatRoom;
    }
}
