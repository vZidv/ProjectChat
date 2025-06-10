using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class RoomHistoryDTO
    {
        public ChatMessageDTO[] MessageDTOs { get; set; }
        public int RoomId { get; set; }
    }
}
