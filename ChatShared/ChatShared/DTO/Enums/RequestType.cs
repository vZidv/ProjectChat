using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO.Enums
{
    public enum RequestType
    {
        Login,
        Register,
        ClientProfileUpdate,

        SendMessage,
        GetHistoryChat,

        SearchChats,

        CreatRoom,
        GetChats,
        UpdateCurrentChatRoom,

        JoimInChatGroup,
        AddContact,
        //..
    }
}
