using ChatClient.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.ViewModels
{
    public class ChatViewModel
    {
        private ChatRoomDTO _chatRoomDTO;

        public ChatRoomDTO ChatRoomDTO
        {
            get { return _chatRoomDTO; }
            set
            {
                _chatRoomDTO = value;
                // NotifyPropertyChanged("ChatRoomDTO"); // Uncomment if using INotifyPropertyChanged
            }
        }

        public ChatViewModel() { }

        public ChatViewModel(ChatRoomDTO chatRoomDTO)
        {
            _chatRoomDTO = chatRoomDTO;
        }
    }
}
