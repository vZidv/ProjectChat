using ChatServer.Data;
using ChatServer.DTO;
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

        public async Task<bool> HandleLoginAsync(ClientLoginDTO loginDTO)
        {
            var user = await _context.Clients.Where(p => p.Login == loginDTO.Login && p.PasswordHash == loginDTO.PasswordHash).FirstOrDefaultAsync();
            if (user != null) return true;

            return false;
        }
    }
}
