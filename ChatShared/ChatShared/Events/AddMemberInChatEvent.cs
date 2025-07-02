using ChatShared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.Events
{
    public class AddMemberInChatEvent : ChatEvent
    {
        public JoinInChatRoomResultDTO JoinInChatRoomResultDTO { get; set; }

        public AddMemberInChatEvent(JoinInChatRoomResultDTO joinInChatRoomResultDTO) : base(DTO.Enums.ResponseType.JoinInChatRoomResult)
        {
            JoinInChatRoomResultDTO = joinInChatRoomResultDTO;
        }
    }
}
