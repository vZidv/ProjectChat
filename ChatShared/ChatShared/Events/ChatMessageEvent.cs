using ChatShared.DTO.Enums;
using ChatShared.DTO.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.Events
{
    public class ChatMessageEvent : ChatEvent
    {
        public MessageDTO Message { get;}

        public ChatMessageEvent(MessageDTO message) : base(ResponseType.Message)
        {
            Message = message;
        }
    }
}
