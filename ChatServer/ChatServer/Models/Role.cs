using System;
using System.Collections.Generic;

namespace ChatServer.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Color { get; set; } = null!;

    public virtual ICollection<RoomMember> RoomMembers { get; set; } = new List<RoomMember>();
}
