using ChatShared.DTO;
using ChatShared.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.Events
{
    public class ChatHistoryEvent : ChatEvent
    {
        public ChatHistoryDTO HistoryDTO { get; }

        public ChatHistoryEvent(ChatHistoryDTO historyDTO ) : base(ResponseType.GetHistoryChatRoom)
        {
            HistoryDTO = historyDTO;
        }
    }
}
