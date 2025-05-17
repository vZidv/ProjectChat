using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public enum ResponseType
    {
        LoginResult,
        SignUpResult,
        CreatRoomResult,
        GetChatRooms,
        Error,
        Message
        //..
    }
}
