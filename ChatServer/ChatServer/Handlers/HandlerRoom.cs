using ChatServer.Data;
using ChatServer.Models;
using ChatShared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ChatServer.Handlers
{
    public class HandlerRoom
    {
        private readonly ProjectChatContext _context;

        public HandlerRoom(ProjectChatContext context)
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
                Password = null,
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

        public async Task<RoomMessageDTO[]> GetHistoryRoomAsync(int roomId)
        {
            ChatRoom room = await _context.ChatRooms.Where(r => r.Id == roomId).Include(r => r.Messages).ThenInclude(m => m.Client).FirstOrDefaultAsync();
            Message[] messages = room.Messages.ToArray();
            var messagesToRoomDTO = new RoomMessageDTO[messages.Length];
            for (int i = 0; i < messages.Length; i++)
            {
                messagesToRoomDTO[i] = new RoomMessageDTO()
                {
                    Text = messages[i].Text,
                    Sender = messages[i].Client.Login,
                    SentAt = messages[i].SentAt,
                    isEdit = messages[i].IsEdited,

                    RoomId = roomId
                };
                
            }
            return messagesToRoomDTO;
        }
    }
}
