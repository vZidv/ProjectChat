using System;
using System.Collections.Generic;

namespace ChatServer.Models;

public partial class ChatRoomMember
{
    public int RoomId { get; set; }

    public int ClientId { get; set; }

    public int RoleId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual ChatRoom Room { get; set; } = null!;
}
