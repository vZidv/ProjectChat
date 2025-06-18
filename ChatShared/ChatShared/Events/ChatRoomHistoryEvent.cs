using ChatShared.DTO;
using ChatShared.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.Events
{
    public class ChatRoomHistoryEvent : ChatEvent
    {
        public RoomHistoryDTO HistoryDTO { get; }

        public ChatRoomHistoryEvent(RoomHistoryDTO roomHistoryDTO ) : base(ResponseType.GetHistoryRoom)
        {
            HistoryDTO = roomHistoryDTO;
        }
    }
}
