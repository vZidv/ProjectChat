using ChatServer.Data;
using ChatServer.Models;
using ChatServer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
