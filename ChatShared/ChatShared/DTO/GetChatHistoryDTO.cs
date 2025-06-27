using ChatShared.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class GetChatHistoryDTO
    {
        public int ChatId { get; set; }
        public ChatType ChatType { get; set; }
        public int Limit { get; set; }
    }
}
