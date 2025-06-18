using System;
using System.Collections.Generic;

namespace ChatServer.Models;

public partial class MessageToChatRoom
{
    public int? ChatRoomId { get; set; }

    public int? MessageId { get; set; }

    public virtual ChatRoom? ChatRoom { get; set; }

    public virtual Message? Message { get; set; }
}
