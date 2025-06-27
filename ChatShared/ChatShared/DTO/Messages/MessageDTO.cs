using ChatShared.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO.Messages
{
    public abstract class MessageDTO
    {
        public MessageType MessageType { get; }

        public string Text { get; set; }
        public string? Sender { get; set; } = null;
        public DateTime? SentAt { get; set; } = null;
        public bool isEdit { get; set; } = false;

        protected MessageDTO() { }

        protected MessageDTO(MessageType messageType)
        {
            MessageType = messageType;
        }
    }
}
