using ChatShared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.Events
{
    public class OpenChatEvent : ChatEvent
    {
        public ChatMiniProfileDTO ChatMiniProfileDTO { get; set; }

        public OpenChatEvent(ChatMiniProfileDTO chatMiniProfileDTO) : base(DTO.Enums.ResponseType.OpenChat) { ChatMiniProfileDTO = chatMiniProfileDTO; }
    }
}
