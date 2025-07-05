using ChatServer.Data;
using ChatServer.Models;
using ChatShared.DTO;
using ChatShared.DTO.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Handlers
{
    public class HandlerChatList
    {
        private readonly ProjectChatContext _context;

        public HandlerChatList(ProjectChatContext context)
        {
            _context = context;
        }

        public async Task<List<ChatMiniProfileDTO>> GetChatListForClientAsync(int clientId)
        {
            List<ChatMiniProfileDTO> result = new();


            Client[] contacts = (await _context.Clients.Where(c => c.Id == clientId).Include(c => c.Contacts).FirstOrDefaultAsync() as Client).Contacts.ToArray();
            ChatRoom[] rooms = await _context.ChatRoomMembers.Where(c => c.ClientId == clientId).Select(c => c.Room).ToArrayAsync();

            result.AddRange(ClientToChatMiniProfileDTO(contacts, true));
            result.AddRange(ChatRoomsToChatMiniProfileDTO(rooms, true));

            return result;
        }

        private List<ChatMiniProfileDTO> ClientToChatMiniProfileDTO(Client[] clients, bool isMember)
        {
            List<ChatMiniProfileDTO> result = new();
            foreach (var client in clients)
            {
                ChatMiniProfileDTO chatMiniProfile = new()
                {
                    Id = client.Id,
                    Name = string.Format($"{client.LastName} {client.Name}"),
                    ChatType = ChatType.Private,

                    IsMember = isMember,


                    LastMessaget = string.Empty, // <- - Replace with actual last message
                    LastActivity = client.LastLogin // <- - Replace with actual last activity
                };
                result.Add(chatMiniProfile);
            }
            return result;
        }

        private List<ChatMiniProfileDTO> ChatRoomsToChatMiniProfileDTO(ChatRoom[] rooms, bool isMember)
        {
            List<ChatMiniProfileDTO> result = new();
            foreach (var room in rooms)
            {
                ChatMiniProfileDTO chatMiniProfile = new()
                {
                    Id = room.Id,
                    Name = room.Name,
                    ChatType = ChatType.Group,
                    IsMember = isMember,
                    LastMessaget = string.Empty, // <- - Replace with actual last message
                    LastActivity = DateTime.Now // <- - Replace with actual last activity
                };
                result.Add(chatMiniProfile);
            }
            return result;
        }

        public async Task<List<ChatMiniProfileDTO>> SeachChatsByNameAsync(string seachName)
        {
            List<ChatMiniProfileDTO> result = new();

            ChatMiniProfileDTO[] rooms = await _context.ChatRooms.Where(c => c.Name.Contains(seachName)).
            Select(c => new ChatMiniProfileDTO
            {
                Id = c.Id,
                Name = c.Name,
                ChatType = ChatType.Group,
                LastMessaget = string.Empty, // <- - Replace with actual last message
                LastActivity = DateTime.Now // <- - Replace with actual last activity
            })
            .ToArrayAsync();

            ChatMiniProfileDTO[] contacts = await _context.Clients.Where(c => 
            ((c.LastName ?? "") + "" + (c.Name ?? "")).Contains(seachName) ||
            (c.Name ?? "").Contains(seachName) ||
            (c.LastName ?? "").Contains(seachName)).
            Select(c => new ChatMiniProfileDTO
            {
                Id = c.Id,
                Name = string.Format($"{c.LastName} {c.Name}"),
                ChatType = ChatType.Private,
                IsContact = false,
                LastMessaget = string.Empty, // <- - Replace with actual last message
                LastActivity = DateTime.Now // <- - Replace with actual last activity
            })
            .ToArrayAsync();

            result.AddRange(rooms);
            result.AddRange(contacts);

            return result;
        }

        public async Task<List<ChatMiniProfileDTO>> SearchNewChatForClientByName(string seachName, int clientId)
        {
            List<ChatMiniProfileDTO> result = await SeachChatsByNameAsync(seachName);

            int[] contactsMember = await _context.Clients.Where(c => c.Id == clientId)
                .SelectMany(c => c.Contacts.Select(c => c.Id)).ToArrayAsync();

            int[] chatRoomMember = await _context.ChatRoomMembers.Where(c => c.ClientId == clientId)
                .Select(c => c.RoomId)
                .ToArrayAsync();

            result = result.Where(c =>
            (c.Id != clientId || c.ChatType != ChatType.Private) &&
            (c.ChatType != ChatType.Group || !chatRoomMember.Contains(c.Id)) &&
            (c.ChatType != ChatType.Private || !contactsMember.Contains(c.Id))).ToList();
            return result;
        }
    }
}
