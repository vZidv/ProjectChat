using ChatShared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatClient.ViewModels
{
    class ChatRoomProfileViewModel : BaseViewModel
    {
        //Fields

        private ChatRoomDTO _chatRoomDTO;

        //Properties

        public ChatRoomDTO ChatRoomDTO
        {
            get { return _chatRoomDTO; }
            set
            {
                _chatRoomDTO = value;
                OnPropertyChanged(nameof(ChatRoomDTO));
            }
        }

        //Commands

        public ICommand ClosePageCommand { get; }

        public ChatRoomProfileViewModel() {}
        
        public ChatRoomProfileViewModel(ChatRoomDTO chatRoomDTO)
        {
            ChatRoomDTO = chatRoomDTO;

            ClosePageCommand = new ViewModelCommand(ExecuteClosePageCommand);
        }

        private void ExecuteClosePageCommand(object? obj) => Services.NavigationService.TopFrame.Content = null;
    }
}
