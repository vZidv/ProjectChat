using ChatServer.Data;
using ChatShared.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Handlers
{
    public class HandleLogin
    {
        private readonly ProjectChatContext _context;

        public HandleLogin(ProjectChatContext context)
        {
            _context = context;
        }

        public async Task<LoginResultDTO> HandleLoginAsync(ClientLoginDTO loginDTO)
        {
            var user = await _context.Clients.Where(p => p.Login == loginDTO.Login).FirstOrDefaultAsync();

            if (user != null && HandlerPassword.VerifyPassword(loginDTO.PasswordHash, user.PasswordHash))
                return new LoginResultDTO()
                {
                    Success = true,

                    ClientProfileDTO = new() 
                    { 
                        Id = user.Id,
                        Login = user.Login,
                        Name = user.Name,
                        LastName = user.LastName,
                        Email = user.Email,
                        Status = user.Status,
                    },


                    Token = Guid.NewGuid().ToString(),
                    ErrorMessage = null,
                };

            return new LoginResultDTO()
            {
                Success = false,
                ClientProfileDTO = null,
                Token = null,
                ErrorMessage = "Неверный логин или пароль",
            };
        }
    }
}
