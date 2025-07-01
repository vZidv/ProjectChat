using System;
using System.Collections.Generic;

namespace ChatServer.Models;

public partial class ChatRoomType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ChatRoom> ChatRooms { get; set; } = new List<ChatRoom>();
}
