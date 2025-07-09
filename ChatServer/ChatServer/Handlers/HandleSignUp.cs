using ChatServer.Data;
using ChatShared.DTO;
using ChatServer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Handlers
{
    public class HandleSignUp
    {
        private readonly ProjectChatContext _context;

        public HandleSignUp(ProjectChatContext context)
        {
            _context = context;
        }

        public async Task<bool> HandleSignUpAsync(ClientSignUpDTO clientSignUpDTO)
        {
            if(clientSignUpDTO == null)
                return false;
            var findUser = await _context.Clients.Where(p => p.Login == clientSignUpDTO.Login || p.Email == clientSignUpDTO.Email).FirstOrDefaultAsync();
            if (findUser != null)
                return false;

            string? filePath = null;
            if (clientSignUpDTO.AvatarBase64 != null)
            {
                 filePath = new HandlerAvatar(_context)
                    .SaveAvatarAsync($"{clientSignUpDTO.Login}{clientSignUpDTO.AvatarExtension}", clientSignUpDTO.AvatarBase64)
                    .Result;
            }
            var newClient = new Client()
            {
                Login = clientSignUpDTO.Login,
                PasswordHash = HandlerPassword.HashPassword(clientSignUpDTO.PasswordHash),

                Email = clientSignUpDTO.Email,
                Name = clientSignUpDTO.Name,
                LastName = clientSignUpDTO.LastName,

                CreatedAt = DateTime.Now,
                LastLogin = DateTime.Now,

                AvatarPath = filePath,

                Status = null,
                IsBanned = false
            };

            _context.Clients.Add(newClient);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
