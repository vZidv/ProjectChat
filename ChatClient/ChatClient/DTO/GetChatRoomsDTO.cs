using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.DTO
{
    class GetChatRoomsDTO : IRequestData
    {
        public int ClientId { get; set; }

        public RequestType RequestType => RequestType.GetChatRooms;
    }
}
