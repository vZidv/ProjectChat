using ChatShared.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO.Messages
{
    public class RoomMessageDTO : MessageDTO
    {
        public int RoomId { get; set; }

        public RoomMessageDTO() : base(MessageType.RoomMessage) { }

    }
}
