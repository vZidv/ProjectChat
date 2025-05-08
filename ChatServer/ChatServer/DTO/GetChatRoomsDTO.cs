using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.DTO
{
    class GetChatRoomsDTO
    {
        public int ClientId { get; set; }

        public RequestType RequestType => RequestType.GetChatRooms;
    }
}
