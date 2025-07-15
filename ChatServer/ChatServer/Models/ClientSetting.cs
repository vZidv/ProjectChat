using System;
using System.Collections.Generic;

namespace ChatServer.Models;

public partial class ClientSetting
{
    public int ClientId { get; set; }

    public string Path { get; set; } = null!;

    public virtual Client Client { get; set; } = null!;
}
