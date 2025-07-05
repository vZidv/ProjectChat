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
    public class HandlerContact
    {
        private readonly ProjectChatContext _context;

        public HandlerContact(ProjectChatContext context)
        {
            _context = context;
        }

        public async Task<AddContactResultDTO> AddContactToClientAsync(int senderClientId, int reciverClientId)
        {
            Client sender = await _context.Clients.FindAsync(senderClientId);
            Client reciver = await _context.Clients.FindAsync(reciverClientId);

            reciver.Clients.Add(sender);
            _context.Update(sender);

            var result = new AddContactResultDTO() { IsSuccess = true};

            try
            { 
                await _context.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                Console.WriteLine($"Не удадлсб добавить пользователя в контакты. Ошибка: {ex.Message}");
                return result;
            }
        }
        }
    }
