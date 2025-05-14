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
                    ClientId = user.Id,
                    Token = Guid.NewGuid().ToString(),
                    ErrorMessage = null,
                };

            return new LoginResultDTO()
            {
                Success = false,
                ClientId = -1,
                Token = null,
                ErrorMessage = "Неверный логин или пароль",
            };
        }
    }
}
