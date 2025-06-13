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
        public int ClientId { get; set; }
        public string Token { get; set; }
        public NetworkStream Stream { get; set; } = null!;
        public int CurrentRoomId { get; set; } = -1;
    }
}
