using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class ChatMessageDTO
    {
        public int RoomId { get; set; }
        public string Text { get; set; }

        public string? Sender { get; set; } = null;
        public DateTime? SentAt { get; set; } = null;
        public bool isEdit { get; set; } = false;
    }
}
