using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class SeachChatResultDTO
    {
        public string SearchText { get; set; }
        public ChatMiniProfileDTO[] Chats { get; set; }
        public int TotalCount { get; set; }
    }
}
