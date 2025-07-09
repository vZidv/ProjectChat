using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class ClientSignUpDTO
    {
        public string Login { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string? Name { get; set; } = null!;
        public string? LastName { get; set; } = null!;

        public string? AvatarBase64 { get; set; } = null!;
        public string? AvatarExtension { get; set; } = null!;
    }
}
