using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.DTO
{
    public enum RequestType
    {
        Login,
        Register,
        SendMessage,
        CreatRoom,
        GetChatRooms
        //..
    }
}
