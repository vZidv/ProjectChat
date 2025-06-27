using ChatServer.Data;
using ChatServer.Models;
using ChatShared.DTO;
using ChatShared.DTO.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ChatShared.DTO.Enums;

namespace ChatServer.Handlers
{
    public class HandlerChatRoom
    {
        private readonly ProjectChatContext _context;

        public HandlerChatRoom(ProjectChatContext context)
        {
            _context = context;
        }

        public async Task<CreatChatRoomResultDTO> CreatRoomAsync(ChatRoomDTO roomDTO)
        {
            CreatChatRoomResultDTO result = new(true, null, roomDTO);
            if (roomDTO == null)
            {
                result.Success = false;
                result.ErrorMessage = "Room data is null.";
                return result;
            }

            var chatRoom = new ChatRoom()
            {
                Name = roomDTO.Name,
                IsPrivate = false,
                OwnerId = roomDTO.OwnerId
            };
            _context.ChatRooms.Add(chatRoom);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            result.ChatRoomDTO.Id = chatRoom.Id;

            return result;
        }

        public async Task<ChatRoomDTO[]> GetRoomsForClientAsync(int clientId)
        {
            ChatRoom[] rooms = await _context.ChatRooms.Where(x => x.IsPrivate == false || x.OwnerId == clientId).ToArrayAsync();
            var roomDTOs = new ChatRoomDTO[rooms.Length];
            for (int i = 0; i < rooms.Length; i++)
            {
                roomDTOs[i] = new ChatRoomDTO()
                {
                    Id = rooms[i].Id,
                    Name = rooms[i].Name,
                    OwnerId = rooms[i].OwnerId
                };
            }
            return roomDTOs;
        }

        public async Task<RoomMessageDTO[]> GetHistoryChatRoomAsync(int chatRoomId, ChatType chatType, int senderId)
        {
            RoomMessageDTO[] result = null!;
            Message[] messages = null!;
            switch (chatType)
            {
                case ChatType.Private:
                    {
                         messages = await _context.Messages
                            .Where(
                            m => (m.ClientId == senderId && m.Clients.Any(c => c.Id == chatRoomId)) ||
                                 (m.ClientId == chatRoomId && m.Clients.Any(c => c.Id == senderId)))
                            .Include(m => m.Client).ToArrayAsync();

                        result = new RoomMessageDTO[messages.Length];

                    }
                    break;

                case ChatType.Group:
                    {
                        ChatRoom room = await _context.ChatRooms.Where(r => r.Id == chatRoomId).Include(r => r.Messages).ThenInclude(m => m.Client).FirstOrDefaultAsync();

                        messages = room.Messages.ToArray();
                        result = new RoomMessageDTO[messages.Length];

                    }
                    break;
            }

            for (int i = 0; i < messages.Length; i++)
            {
                result[i] = new RoomMessageDTO()
                {
                    Text = messages[i].Text,
                    Sender = messages[i].Client.Login,
                    SentAt = messages[i].SentAt,
                    isEdit = messages[i].IsEdited,

                    RoomId = chatRoomId
                };

            }

            return result;
        }
    }
}
