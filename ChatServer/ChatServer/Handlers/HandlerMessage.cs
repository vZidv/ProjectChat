using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatServer.Data;
using ChatServer.Models;
using ChatShared.DTO;
using Microsoft.EntityFrameworkCore;

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



            switch (newMessageDTO.MessageType)
            {
                case MessageType.RoomMessage:
                    {
                        var roomMessageDTO = newMessageDTO as RoomMessageDTO;

                        if ((_context.ChatRooms.FirstOrDefault(r => r.Id == roomMessageDTO.RoomId) is ChatRoom room))
                            newMessage.ChatRooms.Add(room);
                        
                    }
                    break;
                case MessageType.PrivateMessage:
                    {
                        var message = newMessageDTO as PrivateMessageDTO;

                        var messageToChatRoom = new MessageToClient()
                        {
                            ClientId = message.ClientId,
                            MessageId = newMessage.Id
                        };

                    }
                    break;
            }

            await _context.SaveChangesAsync();
            return newMessageDTO;
        }
    }
}
