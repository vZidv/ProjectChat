using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatServer.Data;
using ChatServer.Models;
using ChatShared.DTO;

namespace ChatServer.Handlers
{
    public class HandlerMessage
    {
        private readonly ProjectChatContext _context;
        private HandlerClient handlerClient;

        public HandlerMessage(ProjectChatContext context, HandlerClient handlerClient)
        {
            _context = context;
            this.handlerClient = handlerClient;
        }

        public async Task WritingMessageAsync(ChatMessageDTO newMessageDTO, int clientId)
        {
            var newMessage = new Message()
            {
                Text = newMessageDTO.Text,
                ClientId = clientId,
                RoomId = newMessageDTO.RoomId,
                SentAt = DateTime.Now,
                IsEdited = false
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();
            
        }

        
    }
}
