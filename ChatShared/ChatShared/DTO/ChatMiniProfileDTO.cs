using ChatShared.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class ChatMiniProfileDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ChatType ChatType { get; set; }

        public string LastMessaget { get; set; }
        public DateTime LastActivity { get; set; }

        public bool IsMember { get; set; }
        public bool? IsContact { get; set; }

        public string? AvatarBase64 { get; set; }
        public string? AvatarExtension { get; set; } = null!;
    }
}
