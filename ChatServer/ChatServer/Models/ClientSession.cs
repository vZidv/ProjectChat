using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Models
{
    public class ClientSession
    {
        public int ClientId { get; set; }
        public string Token { get; set; }
        public int CurrentRoomId { get; set; } = -1;
    }
}
