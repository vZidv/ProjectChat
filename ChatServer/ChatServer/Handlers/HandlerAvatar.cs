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
    public class HandlerAvatar
    {
        private readonly ProjectChatContext _context;
        private readonly string _directory = "Avatars";

        public HandlerAvatar(Data.ProjectChatContext context)
        {
            _context = context;
        }

        public async Task<string> SaveAvatarAsync(string name, string base64Image)
        {
            Directory.CreateDirectory(_directory);
            string filePath = $"{_directory}/{name}";

            byte[] avatarBytes = Convert.FromBase64String(base64Image);
            await File.WriteAllBytesAsync(filePath, avatarBytes);

            return filePath;
        }

        public async Task<string> GetClientAvatarAync(int clientId)
        {
            string? result = null;

            string? filePath = await _context.Clients.Where(c => c.Id == clientId).Select(c => c.AvatarPath).FirstOrDefaultAsync();
            if (filePath != null)
                result = ImageToBase64(filePath);
            return result;
        }

        public async Task<string> GetRoomAvatarAync(int roomId)
        {
            string? result = null;

            string? filePath = await _context.ChatRooms.Where(c => c.Id == roomId).Select(c => c.AvatarPath).FirstOrDefaultAsync();
            if (filePath != null)
                result = ImageToBase64(filePath);
            return result;
        }

        public string? ImageToBase64(string filePath)
        {
            if (filePath == null)
                return null;
            byte[] fileBytes = File.ReadAllBytes(filePath);
            return Convert.ToBase64String(fileBytes);
        }
    }
}
