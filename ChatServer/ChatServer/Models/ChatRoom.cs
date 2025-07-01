using System;
using System.Collections.Generic;

namespace ChatServer.Models;

public partial class ChatRoom
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int ChatRoomTypeId { get; set; }

    public int? OwnerId { get; set; }

    public bool IsPrivate { get; set; }

    public virtual ICollection<ChatRoomMember> ChatRoomMembers { get; set; } = new List<ChatRoomMember>();

    public virtual ChatRoomType ChatRoomType { get; set; } = null!;

    public virtual Client? Owner { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
