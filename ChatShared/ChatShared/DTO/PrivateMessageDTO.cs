using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class PrivateMessageDTO : MessageDTO
    {
        public int ClientId { get; set; }

        public PrivateMessageDTO() : base(MessageType.PrivateMessage) {}
    }
}
