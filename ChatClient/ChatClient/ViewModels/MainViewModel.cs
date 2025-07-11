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
using System.Windows;
using MessageBox = ChatClient.CustomControls.MessageBox;
using ControlzEx.Standard;
using Timer = System.Timers.Timer;
using System.Timers;

namespace ChatClient.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        //Fields
        private ChatMiniProfileDTO _selectedChat;

        private ObservableCollection<ChatMiniProfileDTO> _chats;

        private Visibility _allChatsVisibility = Visibility.Visible;
        private Visibility _isSearching = Visibility.Hidden;

        public ObservableCollection<ChatMiniProfileDTO> _localSearchResults;
        public ObservableCollection<ChatMiniProfileDTO> _globalSearchResults;

        private Page _currentPage;

        private string _searchText;
        private Timer _debounceTimer;

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

        public Visibility AllChatsVisibility
        {
            get { return _allChatsVisibility; }
            set
            {
                _allChatsVisibility = value;
                OnPropertyChanged(nameof(AllChatsVisibility));
            }
        }

        public Visibility IsSearching
        {
            get { return _isSearching; }
            set
            {
                _isSearching = value;
                OnPropertyChanged(nameof(IsSearching));
            }
        }

        public ObservableCollection<ChatMiniProfileDTO> LocalSearchResults
        {
            get { return _localSearchResults; }
            set
            {
                _localSearchResults = value;
                OnPropertyChanged(nameof(LocalSearchResults));
            }
        }

        public ObservableCollection<ChatMiniProfileDTO> GlobalSearchResults
        {
            get { return _globalSearchResults; }
            set
            {
                _globalSearchResults = value;
                OnPropertyChanged(nameof(GlobalSearchResults));
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
                UpdateChatsListView(_searchText);
                SearchChatsCommand.Execute(this);

                _debounceTimer.Stop();
                _debounceTimer.Start();
            }
        }

        //Commands
        public ICommand LogoutCommand { get; }
        public ICommand LoadChatsCommand { get; }

        public ICommand OpenLeftBoarMenuCommand { get; }

        public ICommand SearchChatsCommand { get; }


        public MainViewModel()
        {
            LogoutCommand = new ViewModelCommand(ExecuteLogoutCommand);
            LoadChatsCommand = new ViewModelCommand(ExecuteLoadChatsCommand);

            OpenLeftBoarMenuCommand = new ViewModelCommand(ExecuteOpenLeftBoarMenuCommand);

            SearchChatsCommand = new ViewModelCommand(ExecuteSearchChatsCommand, CanExecuteSearchChatsCommand);

            CurrentPage = new View.EmptyChatView();

            LoadChatsCommand.Execute(null);

            App.EventAggregator.Subscribe<CreatRoomEvent>(AddCreatedChatRoom);
            App.EventAggregator.Subscribe<SearchChatsEvent>(onSearchChatGlobal);
            App.EventAggregator.Subscribe<AddMemberInChatEvent>(onJoinInChatRoom);
            App.EventAggregator.Subscribe<UpdateChatRoomProfileEvent>(onUpdateChatRoomProfile);

            _debounceTimer = new Timer(500);
            _debounceTimer.AutoReset = false;
            _debounceTimer.Elapsed += DebounceTimer_Elapsed;
        }


        private bool CanExecuteSearchChatsCommand(object? obj)
        {
            return !string.IsNullOrEmpty(SearchText);
        }


        private void ExecuteSearchChatsCommand(object? obj)
        {
            LocalSearchResults = LocalChatsSearch(SearchText);
        }

        private void DebounceTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            SearchChatsGlobal(SearchText);
        }

        private async void SearchChatsGlobal(string name)
        {
            var request = new SeachChatDTO()
            {
               SearchText = name
            };

            var session = NetworkSession.Session!;
            await session.SendAsync(request, RequestType.SearchChats);
        }

        private void onSearchChatGlobal(SearchChatsEvent searchChatsEvent)
        {
            ChatMiniProfileDTO[] chatMiniProfileDTO = searchChatsEvent.SeachChatResultDTO.Chats;
            GlobalSearchResults = new ObservableCollection<ChatMiniProfileDTO>(chatMiniProfileDTO);
        }
        private ObservableCollection<ChatMiniProfileDTO> LocalChatsSearch(string name)
        {
            return new ObservableCollection<ChatMiniProfileDTO>(Chats.Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToArray());
        }

        void UpdateChatsListView(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                IsSearching = Visibility.Visible;
                AllChatsVisibility = Visibility.Hidden;
            }
            else
            {
                IsSearching = Visibility.Hidden;
                AllChatsVisibility = Visibility.Visible;
            }
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
                await session.SendAsync(SelectedChat, RequestType.UpdateCurrentChatRoom);
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
                ChatType = ChatType.Private,
                LastMessaget = string.Empty, // <- - Replace with actual last message
                LastActivity = DateTime.Now

            };
        }

        private void onJoinInChatRoom(AddMemberInChatEvent @event)
        {
            if(@event.JoinInChatRoomResultDTO == null) return;
            Chats.Add(@event.JoinInChatRoomResultDTO.ChatMiniProfileDTO);
        }

        private void onUpdateChatRoomProfile(UpdateChatRoomProfileEvent @event)
        {
            var data = @event.ChatMiniProfileDTO;
            if (data == null) return;

            var oldProfile = Chats.Where(c => c.Id == data.Id).FirstOrDefault();

            Chats.Remove(oldProfile);
            Chats.Add(data);
        }
    }
}
