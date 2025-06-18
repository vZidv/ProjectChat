using ChatShared.DTO;
using ChatShared.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.Events
{
    public class CreatRoomEvent : ChatEvent
    {
        public CreatChatRoomResultDTO? CreatChatRoomResultDTO { get; }

        public CreatRoomEvent(CreatChatRoomResultDTO creatChatRoomResultDTO) : base(ResponseType.CreatRoomResult) { CreatChatRoomResultDTO = creatChatRoomResultDTO; }

    }
}
