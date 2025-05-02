using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.DTO
{
    public class ClientSignUpDTO
    {
        public string Login { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string Email { get; set; } = null!;

        public RequestType RequestType => RequestType.Register;
    }
}
