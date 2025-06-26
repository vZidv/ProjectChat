using ChatServer.Data;
using ChatServer.Models;
using ChatShared.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Handlers
{
    public class HandlerChat
    {
        private readonly ProjectChatContext _context;

        public HandlerChat(ProjectChatContext context)
        {
            _context = context;
        }

        public async Task<List<ChatMiniProfileDTO>> GetChatListForClientAsync(int clientId)
        {
            List<ChatMiniProfileDTO> result = new();

           
            Client[] contacts = (await _context.Clients.Where(c => c.Id == clientId).Include(c => c.Contacts).FirstOrDefaultAsync() as Client).Contacts.ToArray();
            ChatRoom[] rooms = await _context.ChatRooms.Where(r => r.IsPrivate == false || r.OwnerId == clientId).ToArrayAsync();

            result.AddRange(ClientToChatMiniProfileDTO(contacts));
            result.AddRange(ChatRoomsToChatMiniProfileDTO(rooms));

            return result;
        }

        private List<ChatMiniProfileDTO> ClientToChatMiniProfileDTO(Client[] clients)
        {
            List<ChatMiniProfileDTO> result = new();
            foreach (var client in clients)
            {
                ChatMiniProfileDTO chatMiniProfile = new()
                {
                    Id = client.Id,
                    Name = string.Format($"{client.LastName} {client.Name}"),
                    isGroup = false,
                    LastMessaget = string.Empty, // <- - Replace with actual last message
                    LastActivity = client.LastLogin // <- - Replace with actual last activity
                };
                result.Add(chatMiniProfile);
            }
            return result;
        }

        private List<ChatMiniProfileDTO> ChatRoomsToChatMiniProfileDTO(ChatRoom[] rooms)
        {
            List<ChatMiniProfileDTO> result = new();
            foreach (var room in rooms)
            {
                ChatMiniProfileDTO chatMiniProfile = new()
                {
                    Id = room.Id,
                    Name = room.Name,
                    isGroup = true,
                    LastMessaget = string.Empty, // <- - Replace with actual last message
                    LastActivity = DateTime.Now // <- - Replace with actual last activity
                };
                result.Add(chatMiniProfile);
            }
            return result;
        }
    }
}
