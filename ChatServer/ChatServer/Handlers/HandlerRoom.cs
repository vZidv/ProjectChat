using ChatServer.Data;
using ChatServer.Models;
using ChatServer.DTO;
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

        public async Task<bool> CreatRoomAsync(ChatRoomDTO roomDTO)
        {
            if (roomDTO == null)
                return false;

            var chatRoom = new ChatRoom()
            {
                Name = roomDTO.Name,
                IsPrivate = false,
                Password = null,
                OwnerId = roomDTO.OwnerId
            };
            _context.ChatRooms.Add(chatRoom);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ChatRoomDTO[]> GetRoomsForClientAsync(int clientId)
        {
            ChatRoom[] rooms = await _context.ChatRooms.Where(x => x.IsPrivate == false || x.OwnerId == clientId).ToArrayAsync();
            var roomDTOs = new ChatRoomDTO[rooms.Length];
            for (int i = 0; i < rooms.Length; i++)
            {
                roomDTOs[i] = new ChatRoomDTO()
                {
                    Name = rooms[i].Name,
                    OwnerId = rooms[i].OwnerId
                };
            }
            return roomDTOs;
        }
    }
}
