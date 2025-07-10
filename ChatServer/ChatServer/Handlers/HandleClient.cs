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
    public class HandleClient
    {
        private readonly ProjectChatContext _context;

        public HandleClient(ProjectChatContext context)
        {
            _context = context;
        }

        public async Task<LoginResultDTO> ClientLoginAsync(ClientLoginDTO loginDTO)
        {
            var user = await _context.Clients.Where(p => p.Login == loginDTO.Login).FirstOrDefaultAsync();

            if (user != null && HandlerPassword.VerifyPassword(loginDTO.PasswordHash, user.PasswordHash))
            {
                string? avatarBase64 = null;
                if (!string.IsNullOrWhiteSpace(user.AvatarPath))
                    avatarBase64 = new HandlerAvatar(_context).GetClientAvatarAync(user.Id).Result;

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
                        AvatarBase64 = avatarBase64,
                    },


                    Token = Guid.NewGuid().ToString(),
                    ErrorMessage = null,
                };
            }

            return new LoginResultDTO()
            {
                Success = false,
                ClientProfileDTO = null,
                Token = null,
                ErrorMessage = "Неверный логин или пароль",
            };
        }

        public async Task<bool> ClientProfileUpdateAsync(ClientProfileDTO clientProfileDTO)
        {
            Client client = await _context.Clients.Where(c => c.Id == clientProfileDTO.Id).FirstOrDefaultAsync();

            client.Name = clientProfileDTO.Name;
            client.LastName = clientProfileDTO.LastName;
            client.Email = clientProfileDTO.Email;

            if (clientProfileDTO.AvatarBase64 != null)
                client.AvatarPath = await new HandlerAvatar(_context).SaveAvatarAsync($"{client.Login}{clientProfileDTO.AvatarExtension}", clientProfileDTO.AvatarBase64);

            _context.Clients.Update(client);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось сохранить изменения. Ошибка: {ex.Message}");
            }
            return false;
        }
    }
}
