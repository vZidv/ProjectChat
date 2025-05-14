using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class LoginResultDTO
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public int ClientId { get; set; }
        public string? ErrorMessage { get; set; }

        public LoginResultDTO() { }
        public LoginResultDTO(bool success, string? token, int userId, string errorMessage)
        {
            Success = success;
            Token = token;
            ClientId = userId;
            ErrorMessage = errorMessage;
        }
    }
}
