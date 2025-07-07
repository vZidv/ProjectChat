using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatServer.Data;
using ChatServer.Models;
using ChatShared.DTO;
using ChatShared.DTO.Enums;
using ChatShared.DTO.Messages;
using Microsoft.EntityFrameworkCore;

namespace ChatServer.Handlers
{
    public class HandlerMessage
    {
        private readonly ProjectChatContext _context;

        public HandlerMessage(ProjectChatContext context)
        {
            _context = context;
        }

        public async Task<MessageDTO> WritingMessageAsync(MessageDTO newMessageDTO, int clientId)
        {
            var newMessage = new Message()
            {
                Text = newMessageDTO.Text,
                ClientId = clientId,
                SentAt = DateTime.Now,
                IsEdited = false
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            var roomMessageDTO = newMessageDTO as MessageDTO;

            if ((await _context.ChatRooms.FirstOrDefaultAsync(r => r.Id == roomMessageDTO.RoomId) is ChatRoom room))
                newMessage.ChatRooms.Add(room);

            await _context.SaveChangesAsync();
            return newMessageDTO;
        }
    }
}
