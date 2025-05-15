using ChatClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatShared.DTO;

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

            //StartListeningMessagesAsync();
        }

        private bool CanExecuteSendMessageCommand(object? obj) => (NewMessageText != string.Empty);


        private async void ExecuteSendMessageCommand(object? obj)
        {
            var message = new ChatMessageDTO()
            {
                Text = NewMessageText,
                RoomId = ChatRoomDTO.Id
            };
            var session = NetworkSession.Session;
            session.SendAsync(message, RequestType.SendMessage);

            NewMessageText = string.Empty;
        }

        private async Task StartListeningMessagesAsync()
        {
            try
            {
                await NetworkSession.Session.ListenMessageAsync(OnNewMessage);
            }
            catch (Exception ex){}
        }

        private void OnNewMessage(ChatMessageDTO message)
        {

        }
    }
}
