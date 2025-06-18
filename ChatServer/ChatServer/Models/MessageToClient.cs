using System;
using System.Collections.Generic;

namespace ChatServer.Models;

public partial class MessageToClient
{
    public int ClientId { get; set; }

    public int MessageId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Message Message { get; set; } = null!;
}
