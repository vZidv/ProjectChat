using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.DTO
{
    public class ClientLoginDTO
    {
        public string Login { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;
    }
}
