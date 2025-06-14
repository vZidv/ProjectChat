using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class CreatChatRoomResultDTO
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public ChatRoomDTO? ChatRoomDTO { get; set; }
        public CreatChatRoomResultDTO(bool success, string? errorMessage, ChatRoomDTO? chatRoomDTO)
        {
            Success = success;
            ErrorMessage = errorMessage;
            ChatRoomDTO = chatRoomDTO;
        }
    }
}
