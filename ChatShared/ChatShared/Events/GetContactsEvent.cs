using ChatShared.DTO;
using ChatShared.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.Events
{
    public class GetContactsEvent : ChatEvent
    {
        public ChatMiniProfileDTO[] ChatMiniProfileDTOs {  get; set; }

        public GetContactsEvent(ChatMiniProfileDTO[] chatMiniProfileDTOs) : base(ResponseType.GetContactsResult) { ChatMiniProfileDTOs = chatMiniProfileDTOs; }
    }
}
