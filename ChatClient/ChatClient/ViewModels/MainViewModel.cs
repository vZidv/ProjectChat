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
        private ChatRoomDTO _selectedChatRoom;
        private ObservableCollection<ChatRoomDTO> _chatRooms;
        private ObservableCollection<ChatRoomDTO> _filteredChatRooms;
        private Page _currentPage;

        private string _searchText;

        //Properties

        public ChatRoomDTO SelectedChatRoom
        {
            get
            {
                return _selectedChatRoom;
            }
            set
            {
                _selectedChatRoom = value;
                OnPropertyChanged(nameof(SelectedChatRoom));
                OpenSelectedChatRoom();
            }
        }

        public ObservableCollection<ChatRoomDTO> ChatRooms
        {
            get
            {
                return _chatRooms;
            }
            set
            {
                _chatRooms = value;
                OnPropertyChanged(nameof(ChatRooms));
            }
        }

        public ObservableCollection<ChatRoomDTO> FilteredChatRooms
        {
            get
            {
                return _filteredChatRooms;
            }
            set
            {
                _filteredChatRooms = value;
                OnPropertyChanged(nameof(FilteredChatRooms));
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
        public ICommand LoadChatRoomsCommand { get; }

        public ICommand OpenLeftBoarMenuCommand { get; }


        public MainViewModel()
        {
            LogoutCommand = new ViewModelCommand(ExecuteLogoutCommand);
            LoadChatRoomsCommand = new ViewModelCommand(ExecuteLoadChatRoomsCommand);
            OpenLeftBoarMenuCommand = new ViewModelCommand(ExecuteOpenLeftBoarMenuCommand);

            CurrentPage = new View.EmptyChatView();

            LoadChatRoomsCommand.Execute(null);
            FilteredChatRooms = ChatRooms;

            App.EventAggregator.Subscribe<CreatRoomEvent>(AddCreatedChatRoom);
        }

        private void ExecuteOpenLeftBoarMenuCommand(object? obj)
        {
            LeftBorderMenuView page = new();
            LeftBorderMenuViewModel model = new();

            page.DataContext = model;

            NavigationService.TopFrame.Content = page;

        }

        private async void ExecuteLoadChatRoomsCommand(object? obj)
        {
            var request = new GetChatRoomsDTO()
            {
                ClientId = NetworkSession.ClientProfile.Id
            };

            var session = NetworkSession.Session;
            await session.SendAsync(request, RequestType.GetChatRooms);
            var response = await session.ResponseAsync<ChatRoomDTO[]>();

            if (response != null)
            {
                ChatRooms = new ObservableCollection<ChatRoomDTO>(response);
            }
            else
            {
                MessageBox.Show("Не удалось загрузить комнаты");
            }
            await NetworkSession.Session.ListenAsync();
        }

        private void ExecuteLogoutCommand(object? obj)
        {
            if (NavigationService.MainFrame != null)
                NavigationService.MainFrame.Content = new LoginView();
            
            NetworkSession.Dispose();
        }

        private async void OpenSelectedChatRoom()
        {
            if (SelectedChatRoom == null) return;

            var chatRoomView = new ChatView();
            var chatViewModel = new ChatViewModel(SelectedChatRoom);
            chatRoomView.DataContext = chatViewModel;
            CurrentPage = chatRoomView;

            try
            {
                var session = NetworkSession.Session;
                await session.SendAsync(SelectedChatRoom, RequestType.UpdateCurrentRoom);
            }
            catch (Exception ex) { }

        }

        private void AddCreatedChatRoom(CreatRoomEvent @event)
        {
            CreatChatRoomResultDTO result = @event.CreatChatRoomResultDTO;
            if (result.Success)
            {
                FilteredChatRooms.Add(result.ChatRoomDTO);
            }
        }

        private void ChatRoomFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                FilteredChatRooms = ChatRooms;
            else
            {
                FilteredChatRooms = new ObservableCollection<ChatRoomDTO>(ChatRooms.Where(x => x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
            }
        }
    }
}
