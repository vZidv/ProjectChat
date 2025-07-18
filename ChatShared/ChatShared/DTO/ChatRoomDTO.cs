﻿using ChatShared.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class ChatRoomDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; } = null!;
        public ChatType ChatType { get; set; }
        public int? OwnerId { get; set; } = null!;
        public bool IsPrivate { get; set; }

        public string? AvotarBase64 { get; set; } = null!;
        public string? AvatarExtension { get; set; } = null!;
    }
}
