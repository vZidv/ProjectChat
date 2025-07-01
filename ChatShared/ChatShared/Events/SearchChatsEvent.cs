using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatShared.DTO;

namespace ChatShared.Events
{
    public class SearchChatsEvent : ChatEvent
    {
        public SeachChatResultDTO SeachChatResultDTO { get; }

        public SearchChatsEvent(SeachChatResultDTO seachChatResultDTO) : base(DTO.Enums.ResponseType.SearchChatsResult) { SeachChatResultDTO = seachChatResultDTO; }
    }
}
