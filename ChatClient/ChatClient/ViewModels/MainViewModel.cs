using ChatClient.View;
using Newtonsoft.Json;
using ChatClient.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Controls;
using ChatShared.DTO;
using ChatShared.DTO.Enums;
using ChatClient.Services;
using ChatShared.Events;
using System.Collections.ObjectModel;

namespace ChatClient.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        //Fields
        private ChatMiniProfileDTO _selectedChat;
        private ObservableCollection<ChatMiniProfileDTO> _chats;
        private ObservableCollection<ChatMiniProfileDTO> _filteredChats;
        private Page _currentPage;

        private string _searchText;

        //Properties

        public ChatMiniProfileDTO SelectedChat
        {
            get
            {
                return _selectedChat;
            }
            set
            {
                _selectedChat = value;
                OnPropertyChanged(nameof(SelectedChat));
                OpenSelectedChatRoom();
            }
        }

        public ObservableCollection<ChatMiniProfileDTO> Chats
        {
            get
            {
                return _chats;
            }
            set
            {
                _chats = value;
                OnPropertyChanged(nameof(Chats));
            }
        }

        public ObservableCollection<ChatMiniProfileDTO> FilteredChats
        {
            get
            {
                return _filteredChats;
            }
            set
            {
                _filteredChats = value;
                OnPropertyChanged(nameof(FilteredChats));
            }
        }

        public Page CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                ChatRoomFilter();
            }
        }

        //Commands
        public ICommand LogoutCommand { get; }
        public ICommand LoadChatsCommand { get; }

        public ICommand OpenLeftBoarMenuCommand { get; }


        public MainViewModel()
        {
            LogoutCommand = new ViewModelCommand(ExecuteLogoutCommand);
            LoadChatsCommand = new ViewModelCommand(ExecuteLoadChatsCommand);
            OpenLeftBoarMenuCommand = new ViewModelCommand(ExecuteOpenLeftBoarMenuCommand);

            CurrentPage = new View.EmptyChatView();

            LoadChatsCommand.Execute(null);
            FilteredChats = Chats;

            App.EventAggregator.Subscribe<CreatRoomEvent>(AddCreatedChatRoom);
        }

        private void ExecuteOpenLeftBoarMenuCommand(object? obj)
        {
            LeftBorderMenuView page = new();
            LeftBorderMenuViewModel model = new();

            page.DataContext = model;

            NavigationService.TopFrame!.Content = page;

        }

        private async void ExecuteLoadChatsCommand(object? obj)
        {
            var request = new GetChatsDTO()
            {
                ClientId = NetworkSession.ClientProfile!.Id
            };

            var session = NetworkSession.Session!;
            await session.SendAsync(request, RequestType.GetChats);
            var response = await session.ResponseAsync<ChatMiniProfileDTO[]>();

            if (response != null)
            {
                Chats = new ObservableCollection<ChatMiniProfileDTO>(response);
            }
            else
            {
                MessageBox.Show("Не удалось загрузить комнаты");
            }
            await NetworkSession.Session!.ListenAsync();
        }

        private void ExecuteLogoutCommand(object? obj)
        {
            if (NavigationService.MainFrame != null)
                NavigationService.MainFrame.Content = new LoginView();
            
            NetworkSession.Dispose();
        }

        private async void OpenSelectedChatRoom()
        {
            if (SelectedChat == null) return;

            var chatRoomView = new ChatView();
            var chatViewModel = new ChatViewModel(SelectedChat);
            chatRoomView.DataContext = chatViewModel;
            CurrentPage = chatRoomView;

            try
            {
                var session = NetworkSession.Session;
                await session.SendAsync(SelectedChat, RequestType.UpdateCurrentRoom);
            }
            catch (Exception ex) { }

        }

        private void AddCreatedChatRoom(CreatRoomEvent @event)
        {
            CreatChatRoomResultDTO result = @event.CreatChatRoomResultDTO!;
            ChatMiniProfileDTO newRoom = new()
            {
                Id = result.ChatRoomDTO!.Id,
                Name = result.ChatRoomDTO!.Name,
                isGroup = true,
                LastMessaget = string.Empty, // <- - Replace with actual last message
                LastActivity = DateTime.Now

            };
            if (result.Success)
            {
                FilteredChats.Add(newRoom);
            }
        }

        private void ChatRoomFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                FilteredChats = Chats;
            else
            {
                FilteredChats = new ObservableCollection<ChatMiniProfileDTO>(Chats.Where(x => x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
            }
        }
    }
}
