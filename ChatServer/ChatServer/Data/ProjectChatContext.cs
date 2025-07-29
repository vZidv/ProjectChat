using System;
using System.Collections.Generic;
using ChatServer.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatServer.Data;

public partial class ProjectChatContext : DbContext
{
    public ProjectChatContext()
    {
    }

    public ProjectChatContext(DbContextOptions<ProjectChatContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChatRoom> ChatRooms { get; set; }

    public virtual DbSet<ChatRoomMember> ChatRoomMembers { get; set; }

    public virtual DbSet<ChatRoomType> ChatRoomTypes { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientSetting> ClientSettings { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"); 
            // ?? "Server=DESKTOP-2BSAL1V\\SQLEXPRESS;Database=ProjectChat;Trusted_Connection=True;TrustServerCertificate=True";
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChatRoom__3214EC077CC9DB78");

            entity.ToTable("ChatRoom");

            entity.HasIndex(e => e.OwnerId, "IX_ChatRoom_OwnerId");

            entity.Property(e => e.AvatarPath).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.ChatRoomType).WithMany(p => p.ChatRooms)
                .HasForeignKey(d => d.ChatRoomTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChatRoom_ChatRoomType");

            entity.HasOne(d => d.Owner).WithMany(p => p.ChatRooms)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK__ChatRoom__OwnerI__4D94879B");

            entity.HasMany(d => d.Messages).WithMany(p => p.ChatRooms)
                .UsingEntity<Dictionary<string, object>>(
                    "MessageToChatRoom",
                    r => r.HasOne<Message>().WithMany()
                        .HasForeignKey("MessageId")
                        .HasConstraintName("FK_MessageToChatRoom_Message"),
                    l => l.HasOne<ChatRoom>().WithMany()
                        .HasForeignKey("ChatRoomId")
                        .HasConstraintName("FK_MessageToChatRoom_ChatRoom"),
                    j =>
                    {
                        j.HasKey("ChatRoomId", "MessageId");
                        j.ToTable("MessageToChatRoom");
                        j.HasIndex(new[] { "MessageId" }, "IX_MessageToChatRoom_MessageId");
                    });
        });

        modelBuilder.Entity<ChatRoomMember>(entity =>
        {
            entity.HasKey(e => new { e.RoomId, e.ClientId }).HasName("PK__RoomMemb__6CE1D89B7B89B302");

            entity.ToTable("ChatRoomMember");

            entity.HasIndex(e => e.ClientId, "IX_RoomMember_ClientId");

            entity.HasIndex(e => e.RoleId, "IX_RoomMember_RoleId");

            entity.HasOne(d => d.Client).WithMany(p => p.ChatRoomMembers)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RoomMembe__Clien__5165187F");

            entity.HasOne(d => d.Role).WithMany(p => p.ChatRoomMembers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RoomMembe__RoleI__52593CB8");

            entity.HasOne(d => d.Room).WithMany(p => p.ChatRoomMembers)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RoomMembe__RoomI__5070F446");
        });

        modelBuilder.Entity<ChatRoomType>(entity =>
        {
            entity.ToTable("ChatRoomType");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Client__3214EC0730293144");

            entity.ToTable("Client");

            entity.HasIndex(e => e.Login, "UQ__Client__5E55825BB67C2F1F").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Client__A9D10534E31C2524").IsUnique();

            entity.Property(e => e.AvatarPath).HasMaxLength(256);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.LastLogin)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasMany(d => d.Clients).WithMany(p => p.Contacts)
                .UsingEntity<Dictionary<string, object>>(
                    "ClientContact",
                    r => r.HasOne<Client>().WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ClientContact_Client"),
                    l => l.HasOne<Client>().WithMany()
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ClientContact_Client1"),
                    j =>
                    {
                        j.HasKey("ClientId", "ContactId");
                        j.ToTable("ClientContact");
                        j.HasIndex(new[] { "ContactId" }, "IX_ClientContact_ContactId");
                    });

            entity.HasMany(d => d.Contacts).WithMany(p => p.Clients)
                .UsingEntity<Dictionary<string, object>>(
                    "ClientContact",
                    r => r.HasOne<Client>().WithMany()
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ClientContact_Client1"),
                    l => l.HasOne<Client>().WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ClientContact_Client"),
                    j =>
                    {
                        j.HasKey("ClientId", "ContactId");
                        j.ToTable("ClientContact");
                        j.HasIndex(new[] { "ContactId" }, "IX_ClientContact_ContactId");
                    });
        });

        modelBuilder.Entity<ClientSetting>(entity =>
        {
            entity.HasKey(e => e.ClientId);

            entity.Property(e => e.ClientId).ValueGeneratedNever();
            entity.Property(e => e.Path).HasMaxLength(255);

            entity.HasOne(d => d.Client).WithOne(p => p.ClientSetting)
                .HasForeignKey<ClientSetting>(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClientSettings_Client");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Message__3214EC07A5250A36");

            entity.ToTable("Message");

            entity.HasIndex(e => e.ClientId, "IX_Message_ClientId");

            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Text).HasMaxLength(4000);

            entity.HasOne(d => d.Client).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Message__ClientI__5535A963");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC07300B7F7D");

            entity.ToTable("Role");

            entity.HasIndex(e => e.Name, "UQ__Role__737584F660CC8A7D").IsUnique();

            entity.Property(e => e.Color)
                .HasMaxLength(7)
                .HasDefaultValue("#FFFFFF");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
