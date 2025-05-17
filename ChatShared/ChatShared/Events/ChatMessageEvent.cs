using ChatShared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.Events
{
    public class ChatMessageEvent : ChatEvent
    {
        public ChatMessageDTO Message { get;}

        public ChatMessageEvent(ChatMessageDTO message) : base(ResponseType.Message)
        {
            Message = message;
        }
    }
}
