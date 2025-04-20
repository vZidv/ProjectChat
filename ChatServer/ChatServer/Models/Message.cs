using System;
using System.Collections.Generic;

namespace ChatServer.Models;

public partial class Message
{
    public int Id { get; set; }

    public string Text { get; set; } = null!;

    public int ClientId { get; set; }

    public int RoomId { get; set; }

    public DateTime SentAt { get; set; }

    public bool IsEdited { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual ChatRoom Room { get; set; } = null!;
}
