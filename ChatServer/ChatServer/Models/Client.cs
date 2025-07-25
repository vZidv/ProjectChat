﻿using System;
using System.Collections.Generic;

namespace ChatServer.Models;

public partial class Client
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Name { get; set; }

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime LastLogin { get; set; }

    public string? AvatarPath { get; set; }

    public string? Status { get; set; }

    public bool IsBanned { get; set; }

    public virtual ICollection<ChatRoomMember> ChatRoomMembers { get; set; } = new List<ChatRoomMember>();

    public virtual ICollection<ChatRoom> ChatRooms { get; set; } = new List<ChatRoom>();

    public virtual ClientSetting? ClientSetting { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual ICollection<Client> Contacts { get; set; } = new List<Client>();
}
