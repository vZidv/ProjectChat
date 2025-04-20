using System;
using System.Collections.Generic;

namespace ChatServer.Models;

public partial class ChatRoom
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Password { get; set; }

    public bool IsPrivate { get; set; }

    public int OwnerId { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual Client Owner { get; set; } = null!;

    public virtual ICollection<RoomMember> RoomMembers { get; set; } = new List<RoomMember>();
}
