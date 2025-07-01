using ChatShared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Session
{
    public class ClientSession
    {
        public ClientProfileDTO Client { get; set; }
        public string Token { get; set; }
        public NetworkStream Stream { get; set; } = null!;
    }
}
