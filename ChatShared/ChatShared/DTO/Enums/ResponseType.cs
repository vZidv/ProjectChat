using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO.Enums
{
    public enum ResponseType
    {
        LoginResult,
        SignUpResult,

        CreatRoomResult,
        GetChats,
        Error,
        Message,
        GetHistoryChatRoom,
        SearchChatsResult,

        JoinInChatRoomResult,
        AddContactResult
        //..
    }
}
