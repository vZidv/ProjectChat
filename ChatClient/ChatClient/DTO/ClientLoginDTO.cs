using ChatClient.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.DTO
{
    public class ClientLoginDTO : IRequestData
    {
        public int Id { get; set; }

        public string Login { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public RequestType RequestType => RequestType.Login;
    }
}
