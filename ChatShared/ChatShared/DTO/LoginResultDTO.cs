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
        public ClientProfileDTO? ClientProfileDTO { get; set; }
        public string? ErrorMessage { get; set; }

        public LoginResultDTO() { }
        public LoginResultDTO(bool success, string? token, ClientProfileDTO clientProfileDTO, string errorMessage)
        {
            Success = success;
            Token = token;
            ClientProfileDTO = clientProfileDTO;
            ErrorMessage = errorMessage;
        }
    }
}
