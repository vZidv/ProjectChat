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

            ChatRoom[] rooms = await _context.ChatRoomMembers.Where(c => c.ClientId == clientId).Select(c => c.Room).ToArrayAsync();

            result.AddRange(ChatRoomsToChatMiniProfileDTO(rooms, true, clientId));

            return result;
        }
        
        public async Task<ChatMiniProfileDTO[]> GetPrivatChatListForClientAsync(int clientId)
        {
            var result =  GetChatListForClientAsync(clientId).Result.Where(c => c.ChatType == ChatType.Private).ToArray();
            return result;
            
        }

        private List<ChatMiniProfileDTO> ChatRoomsToChatMiniProfileDTO(ChatRoom[] rooms, bool isMember, int clientId)
        {
            List<ChatMiniProfileDTO> result = new();
            var avatarHandler = new HandlerAvatar(_context);
            foreach (var room in rooms)
            {

                ChatMiniProfileDTO chatMiniProfile = new()
                {
                    Id = room.Id,
                    Name = room.Name,
                    IsMember = isMember,
                    LastMessaget = string.Empty, // <- - Replace with actual last message
                    LastActivity = DateTime.Now // <- - Replace with actual last activity
                };

                switch (room.ChatRoomTypeId)
                {
                    case 1: // Group chat
                        { 
                            chatMiniProfile.ChatType = ChatType.Group;
                            chatMiniProfile.AvatarBase64 = avatarHandler.ImageToBase64(room.AvatarPath);
                        }
                        break;
                    case 2: // Private chat
                        {
                            chatMiniProfile.ChatType = ChatType.Private;

                            Client client = _context.ChatRoomMembers.
                                Where(c => c.RoomId == room.Id && c.ClientId != clientId).
                                Select(c => c.Client).FirstOrDefault();

                            chatMiniProfile.AvatarBase64 = avatarHandler.GetClientAvatarAync(client.Id).Result;
                            chatMiniProfile.Name = string.Format($"{client!.LastName} {client!.Name}");
                        }
                        break;
                    default:
                        break;
                }


                result.Add(chatMiniProfile);
            }
            return result;
        }

        public async Task<List<ChatMiniProfileDTO>> SeachChatsByNameAsync(string seachName)
        {
            List<ChatMiniProfileDTO> result = new();
            HandlerAvatar handlerAvatar = new(_context);

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
                LastActivity = DateTime.Now, // <- - Replace with actual last activity

            })
            .ToArrayAsync();


            foreach (ChatMiniProfileDTO room in rooms)
                room.AvatarBase64 = await handlerAvatar.GetRoomAvatarAync(room.Id);
            foreach (ChatMiniProfileDTO contact in contacts)
                contact.AvatarBase64 = await handlerAvatar.GetClientAvatarAync(contact.Id);

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
