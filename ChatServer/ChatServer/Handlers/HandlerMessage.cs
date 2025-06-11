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
        private readonly HandlerClient _handlerClient;

        public HandlerMessage(ProjectChatContext context, HandlerClient handlerClient)
        {
            _context = context;
            this._handlerClient = handlerClient;
        }

        public async Task<ChatMessageDTO> WritingMessageAsync(ChatMessageDTO newMessageDTO, int clientId)
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

            return new ChatMessageDTO()
            {
                Text = newMessageDTO.Text,
                RoomId = newMessageDTO.RoomId,

                Sender = _context.Clients
                    .Where(c => c.Id == clientId)
                    .Select(c => c.Login)
                    .FirstOrDefault(),
                SentAt = newMessage.SentAt,
                isEdit = false
            };

        }

        
    }
}
