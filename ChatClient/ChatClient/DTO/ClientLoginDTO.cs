using ChatClient.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.DTO
{
    public class ClientLoginDTO : IRequestData
    {
        public string Login { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public RequestType RequestType => RequestType.Login;
    }
}
