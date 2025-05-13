using ChatClient.DTO;
using ChatClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatClient.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private ChatRoomDTO _chatRoomDTO;

        private ChatMessageDTO[] _chatMessageDTOs;

        private ClientLoginDTO _clientLoginDTO;

        private string _newMessageText;

        public ChatRoomDTO ChatRoomDTO
        {
            get { return _chatRoomDTO; }
        }

        public ChatMessageDTO[] ChatMessageDTOs
        {
            get { return _chatMessageDTOs; }
            set
            {
                _chatMessageDTOs = value;
                OnPropertyChanged(nameof(ChatMessageDTOs));
            }
        }

        public string NewMessageText
        {
            get { return _newMessageText; }
            set
            {
                _newMessageText = value;
                OnPropertyChanged(nameof(NewMessageText));
            }
        }
        
        //Command
        public ICommand SendMessageCommand { get; }

        public ChatViewModel() { }

        public ChatViewModel(ChatRoomDTO chatRoomDTO, ClientLoginDTO client)
        {
            _chatRoomDTO = chatRoomDTO;

            _clientLoginDTO = client;

            SendMessageCommand = new ViewModelCommand(ExecuteSendMessageCommand, CanExecuteSendMessageCommand);
        }

        private bool CanExecuteSendMessageCommand(object? obj) => (NewMessageText != string.Empty);


        private async void ExecuteSendMessageCommand(object? obj)
        {
            var message = new ChatMessageDTO
            {
                Text = NewMessageText,
                ClientId = _clientLoginDTO.Id,
                RoomId = _chatRoomDTO.Id
            };

            var service = new NetworkService();
            await service.ConnectAsync();
            await service.SendAsync(message);
            ChatMessageDTOs = await service.ResponseAsync<ChatMessageDTO[]>();
        }
    }
}
