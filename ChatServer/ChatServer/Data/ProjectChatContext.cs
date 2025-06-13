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

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoomMember> RoomMembers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-2BSAL1V\\SQLEXPRESS;Database=ProjectChat;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChatRoom__3214EC077CC9DB78");

            entity.ToTable("ChatRoom");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(256);

            entity.HasOne(d => d.Owner).WithMany(p => p.ChatRooms)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChatRoom__OwnerI__4D94879B");
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
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Message__3214EC07A5250A36");

            entity.ToTable("Message");

            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Client).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Message__ClientI__5535A963");

            entity.HasOne(d => d.Room).WithMany(p => p.Messages)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Message__RoomId__5629CD9C");
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

        modelBuilder.Entity<RoomMember>(entity =>
        {
            entity.HasKey(e => new { e.RoomId, e.ClientId }).HasName("PK__RoomMemb__6CE1D89B7B89B302");

            entity.ToTable("RoomMember");

            entity.HasOne(d => d.Client).WithMany(p => p.RoomMembers)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RoomMembe__Clien__5165187F");

            entity.HasOne(d => d.Role).WithMany(p => p.RoomMembers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RoomMembe__RoleI__52593CB8");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomMembers)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RoomMembe__RoomI__5070F446");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
