using ChatClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatShared.DTO;
using ChatShared.DTO.Enums;
using ChatShared.Events;
using System.Windows;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;

namespace ChatClient.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private ChatRoomDTO _chatRoomDTO;
        private ObservableCollection<MessageDTO> _chatMessageDTOs;
        private string _newMessageText;


        public ChatRoomDTO ChatRoomDTO
        {
            get { return _chatRoomDTO; }
        }

        public ObservableCollection<MessageDTO> ChatMessageDTOs
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
                (SendMessageCommand as ViewModelCommand)?.RaiseCanExecuteChanged();
            }
        }



        //Command
        public ICommand SendMessageCommand { get; }
        public ICommand GetMessageCommand { get; }

        public ChatViewModel() { }

        public ChatViewModel(ChatRoomDTO chatRoomDTO)
        {
            _chatRoomDTO = chatRoomDTO;

            SendMessageCommand = new ViewModelCommand(ExecuteSendMessageCommand, CanExecuteSendMessageCommand);
            GetMessageCommand = new ViewModelCommand(ExecuteGetMessageCommand);

            App.EventAggregator.Subscribe<ChatMessageEvent>(OnNewMessageReceived);
            App.EventAggregator.Subscribe<ChatRoomHistoryEvent>(onRoomHistoryReceived);

            ChatMessageDTOs = new();
            GetMessageCommand.Execute(null);
        }

        private async void ExecuteGetMessageCommand(object? obj)
        {
            GetRoomHistoryDTO historyDTO = new()
            {
                RoomId = ChatRoomDTO.Id,
                Limit = 50
            };

            var session = NetworkSession.Session;
            await session.SendAsync(historyDTO, RequestType.GetHistoryRoom);
        }

        private bool CanExecuteSendMessageCommand(object? obj) => (NewMessageText != string.Empty);


        private async void ExecuteSendMessageCommand(object? obj)
        {
            var message = new RoomMessageDTO()
            {
                Text = NewMessageText,
                RoomId = ChatRoomDTO.Id,
                Sender = NetworkSession.ClientProfile.Login
            };
            MessageDTO messageDTO = message;
            var session = NetworkSession.Session;
            await session.SendAsync(messageDTO, RequestType.SendMessage);

            ChatMessageDTOs.Add(message);
            NewMessageText = string.Empty;
        }

        private void onRoomHistoryReceived(ChatRoomHistoryEvent chatRoomHistoryEvent)
        {
            if (chatRoomHistoryEvent.HistoryDTO.RoomId != ChatRoomDTO.Id)
                return;

            ChatMessageDTOs = new ObservableCollection<MessageDTO>(chatRoomHistoryEvent.HistoryDTO.MessageDTOs);
        }



        private void OnNewMessageReceived(ChatMessageEvent chatEvent)
        {
            if((chatEvent.Message as RoomMessageDTO).RoomId != ChatRoomDTO.Id) return;
            ChatMessageDTOs.Add(chatEvent.Message);
        }
    }
}
