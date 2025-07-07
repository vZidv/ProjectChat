using ChatShared.DTO.Enums;
using ChatShared.DTO.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class ChatHistoryDTO
    {
        public int ChatId { get; set; }
        public ChatType ChatType { get; set; }
        public MessageDTO[] MessageDTOs { get; set; }
    }
}
