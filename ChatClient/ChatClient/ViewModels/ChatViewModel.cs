using ChatClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatShared.DTO;
using ChatShared.DTO.Enums;
using ChatShared.DTO.Messages;
using ChatShared.Events;
using System.Windows;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using ChatClient.View;

namespace ChatClient.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private ChatMiniProfileDTO _chatDTO;

        private ObservableCollection<MessageDTO> _chatMessageDTOs;
        private string _newMessageText;
        private CreatMessageDelegate creatMessage;

        //Button visibility
        private Visibility _joinChatGroupButtonVisibility = Visibility.Hidden;
        private Visibility _addContacntButtonVisibility = Visibility.Hidden;

        public ChatMiniProfileDTO ChatDTO
        {
            get { return _chatDTO; }
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

        public Visibility JoinChatGroupButtonVisibility
        {
            get { return _joinChatGroupButtonVisibility; }
            set
            {
                _joinChatGroupButtonVisibility = value;
                OnPropertyChanged(nameof(JoinChatGroupButtonVisibility));
            }
        }

        public Visibility AddContacntButtonVisibility
        {
            get { return _addContacntButtonVisibility; }
            set
            {
                _addContacntButtonVisibility = value;
                OnPropertyChanged(nameof(AddContacntButtonVisibility));
            }
        }



        //Command
        public ICommand SendMessageCommand { get; }
        public ICommand GetMessageCommand { get; }
        public ICommand OpenChatRoomPageCommand { get; }

        public ICommand JoinInChatGroupCommand { get; }
        public ICommand AddContactCommand { get; }

        public ChatViewModel() { }

        public ChatViewModel(ChatMiniProfileDTO chatDTO)
        {
            _chatDTO = chatDTO;

            SendMessageCommand = new ViewModelCommand(ExecuteSendMessageCommand, CanExecuteSendMessageCommand);
            GetMessageCommand = new ViewModelCommand(ExecuteGetMessageCommand);
            //OpenChatRoomPageCommand = new ViewModelCommand(ExecuteOpenChatRoomPageCommand);
            JoinInChatGroupCommand = new ViewModelCommand(ExecuteJoinInChatGroupCommand);
            AddContactCommand = new ViewModelCommand(ExecuteAddContactCommand);

            App.EventAggregator.Subscribe<ChatMessageEvent>(OnNewMessageReceived);
            App.EventAggregator.Subscribe<ChatHistoryEvent>(onChatRoomHistoryReceived);
            App.EventAggregator.Subscribe<AddMemberInChatEvent>(onAddMemberInChatRoom);
            App.EventAggregator.Subscribe<AddContactEvent>(onAddContact);

            ChatInit();
        }


        // Delegates

        private delegate MessageDTO CreatMessageDelegate(string text);

        private void onAddContact(AddContactEvent @event)
        {
            AddContactResultDTO result = @event.AddContactResultDTO;

            if (!result.IsSuccess)
                return;

            AddContacntButtonVisibility = Visibility.Hidden;
        }

        private void ExecuteAddContactCommand(object? obj)
        {
            var request = new AddContactDTO()
            {
                SenderClientId = NetworkSession.ClientProfile.Id,
                ReceiverClientId = ChatDTO.Id
            };

            var session = NetworkSession.Session;
            session.SendAsync(request, RequestType.AddContact);
        }

        private void ChatInit()
        {   CheckButtonsVisiability();
            SetChatRoomType();
            ChatMessageDTOs = new();
            //GetMessageCommand.Execute(null);
        }
        private async void ExecuteJoinInChatGroupCommand(object? obj)
        {
            var request = new JoinInChatRoomDTO()
            {
                ChatRoomId = ChatDTO.Id,
                ClientId = NetworkSession.ClientProfile.Id
            };

            var session = NetworkSession.Session;
            await session.SendAsync(request, RequestType.JoimInChatGroup);
        }

        private void onAddMemberInChatRoom(AddMemberInChatEvent @event)
        {
            JoinInChatRoomResultDTO result = @event.JoinInChatRoomResultDTO;
            if (!result.IsSuccess)
                return;

            ChatDTO.IsMember = true;
            JoinChatGroupButtonVisibility = Visibility.Hidden;
        }

        private void CheckButtonsVisiability()
        {
            _addContacntButtonVisibility = Visibility.Hidden;
            _joinChatGroupButtonVisibility = Visibility.Hidden;

            switch (ChatDTO.ChatType)
            {
                case ChatType.Group:
                    {
                        if(!ChatDTO.IsMember)
                            _joinChatGroupButtonVisibility = Visibility.Visible;
                    }
                    break;
                case ChatType.Private:
                    {
                        if(ChatDTO.IsContact != null && ChatDTO.IsContact == false)
                            _addContacntButtonVisibility = Visibility.Visible;
                    }
                    break;
                default:
                    throw new Exception("Unknown chat type");
            }
        }

        private void SetChatRoomType()
        {
            switch (ChatDTO.ChatType)
            {
                case ChatType.Group:
                    creatMessage = CreatRoomMessageDTO;
                    break;
                case ChatType.Private:
                    creatMessage = CreatPrivateMessageDTO;
                    break;
                default:
                    throw new Exception("Unknown chat type");
            }
        }

        //private void ExecuteOpenChatRoomPageCommand(object? obj)
        //{
        //    ChatRoomProfileView view = new();
        //    ChatRoomProfileViewModel viewModel = new(ChatRoomDTO);
        //    view.DataContext = viewModel;

        //    Services.NavigationService.TopFrame.Content = view;
        //}

        private async void ExecuteGetMessageCommand(object? obj)
        {
            GetChatHistoryDTO historyDTO = new()
            {
                ChatId = ChatDTO.Id,
                ChatType = ChatDTO.ChatType,
                Limit = 50
            };

            var session = NetworkSession.Session;
            await session.SendAsync(historyDTO, RequestType.GetHistoryChat);
        }

        private bool CanExecuteSendMessageCommand(object? obj) => (NewMessageText != string.Empty);


        private async void ExecuteSendMessageCommand(object? obj)
        {
            MessageDTO message = creatMessage(NewMessageText);

            var session = NetworkSession.Session;
            await session.SendAsync(message, RequestType.SendMessage);

            ChatMessageDTOs.Add(message);
            NewMessageText = string.Empty;
        }


        private RoomMessageDTO CreatRoomMessageDTO(string text)
        {
            if (ChatDTO.ChatType != ChatType.Group)
                throw new Exception("Unavailable type");

            return new RoomMessageDTO
            {
                Text = text,
                RoomId = ChatDTO.Id,
                Sender = NetworkSession.ClientProfile.Login
            };
        }

        private PrivateMessageDTO CreatPrivateMessageDTO(string text)
        {
            if (ChatDTO.ChatType != ChatType.Private)
                throw new Exception("Unavailable type");
            return new PrivateMessageDTO
            {
                Text = text,
                ClientId = ChatDTO.Id,
                Sender = NetworkSession.ClientProfile.Login
            };
        }

        private void onChatRoomHistoryReceived(ChatHistoryEvent chatRoomHistoryEvent)
        {
            if (chatRoomHistoryEvent.HistoryDTO.ChatId != ChatDTO.Id || ChatDTO.ChatType != chatRoomHistoryEvent.HistoryDTO.ChatType)
                return;

            ChatMessageDTOs = new ObservableCollection<MessageDTO>(chatRoomHistoryEvent.HistoryDTO.MessageDTOs);
        }



        private void OnNewMessageReceived(ChatMessageEvent chatEvent)
        {
            if (chatEvent.ChatType != ChatDTO.ChatType)
                return;

            switch (chatEvent.ChatType)
            {
                case ChatType.Group:
                    {
                        if ((chatEvent.Message as RoomMessageDTO).RoomId != ChatDTO.Id) return;
                    }
                    break;
                case ChatType.Private:
                    {
                        if ((chatEvent.Message as PrivateMessageDTO).ClientId != ChatDTO.Id) return;
                    }
                    break;
            }

            ChatMessageDTOs.Add(chatEvent.Message);
        }
    }
}
