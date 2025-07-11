using ChatShared.DTO;
using ChatShared.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.Events
{
    public class UpdateChatRoomProfileEvent : ChatEvent
    {
        public ChatMiniProfileDTO ChatMiniProfileDTO { get; set; }

        public UpdateChatRoomProfileEvent(ChatMiniProfileDTO chatMiniProfileDTO) : base(ResponseType.UpdateChatRoomProfileResult)
        {
            ChatMiniProfileDTO = chatMiniProfileDTO;
        }

    }
}
