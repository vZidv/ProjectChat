using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatServer.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatRoomType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRoomType", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ChatRoomType",
                column: "Name",
                values: new object[]
                {
                   "Group",
                   "Private"
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    LastLogin = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    AvatarPath = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsBanned = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Client__3214EC0730293144", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false, defaultValue: "#FFFFFF")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__3214EC07300B7F7D", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Name", "Color" },
                values: new object[,]
                {
                    { "Member", "1" },
                    { "Admin", "2" },
                    { "Owner", "3" }
                });

            migrationBuilder.CreateTable(
                name: "ChatRoom",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChatRoomTypeId = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: true),
                    AvatarPath = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChatRoom__3214EC077CC9DB78", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatRoom_ChatRoomType",
                        column: x => x.ChatRoomTypeId,
                        principalTable: "ChatRoomType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__ChatRoom__OwnerI__4D94879B",
                        column: x => x.OwnerId,
                        principalTable: "Client",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClientContact",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ContactId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientContact", x => new { x.ClientId, x.ContactId });
                    table.ForeignKey(
                        name: "FK_ClientContact_Client",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientContact_Client1",
                        column: x => x.ContactId,
                        principalTable: "Client",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClientSettings",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSettings", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_ClientSettings_Client",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    IsEdited = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Message__3214EC07A5250A36", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Message__ClientI__5535A963",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChatRoomMember",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RoomMemb__6CE1D89B7B89B302", x => new { x.RoomId, x.ClientId });
                    table.ForeignKey(
                        name: "FK__RoomMembe__Clien__5165187F",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__RoomMembe__RoleI__52593CB8",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__RoomMembe__RoomI__5070F446",
                        column: x => x.RoomId,
                        principalTable: "ChatRoom",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MessageToChatRoom",
                columns: table => new
                {
                    ChatRoomId = table.Column<int>(type: "int", nullable: false),
                    MessageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageToChatRoom", x => new { x.ChatRoomId, x.MessageId });
                    table.ForeignKey(
                        name: "FK_MessageToChatRoom_ChatRoom",
                        column: x => x.ChatRoomId,
                        principalTable: "ChatRoom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageToChatRoom_Message",
                        column: x => x.MessageId,
                        principalTable: "Message",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatRoom_ChatRoomTypeId",
                table: "ChatRoom",
                column: "ChatRoomTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRoom_OwnerId",
                table: "ChatRoom",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomMember_ClientId",
                table: "ChatRoomMember",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomMember_RoleId",
                table: "ChatRoomMember",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "UQ__Client__5E55825BB67C2F1F",
                table: "Client",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Client__A9D10534E31C2524",
                table: "Client",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientContact_ContactId",
                table: "ClientContact",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ClientId",
                table: "Message",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageToChatRoom_MessageId",
                table: "MessageToChatRoom",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "UQ__Role__737584F660CC8A7D",
                table: "Role",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatRoomMember");

            migrationBuilder.DropTable(
                name: "ClientContact");

            migrationBuilder.DropTable(
                name: "ClientSettings");

            migrationBuilder.DropTable(
                name: "MessageToChatRoom");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "ChatRoom");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "ChatRoomType");

            migrationBuilder.DropTable(
                name: "Client");
        }
    }
}
