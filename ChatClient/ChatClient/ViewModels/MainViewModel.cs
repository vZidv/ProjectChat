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
using ChatClient.Services;

namespace ChatClient.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        //Fields
        private ClientLoginDTO _clientDTO;

        private ChatRoomDTO _selectedChatRoom; 
        private ChatRoomDTO[] _chatRooms; 
        private ChatRoomDTO[] _filteredChatRooms; 
        private Page _currentPage; 

        private string _searchText;

        //Properties
        public ClientLoginDTO ClientDTO
        {
            get
            {
                return _clientDTO;
            }
            set
            {
                _clientDTO = value;
                OnPropertyChanged(nameof(ClientDTO));
            }
        }

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

        public ChatRoomDTO[] ChatRooms
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

        public ChatRoomDTO[] FilteredChatRooms
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
        public ICommand CreatRoomCommand { get; }
        public ICommand LoadChatRoomsCommand { get; }

        public ICommand OpenLeftBoarMenuCommand { get; }

        public MainViewModel()
        {

        }
        public MainViewModel(ClientLoginDTO clientDTO)
        {
            ClientDTO = clientDTO;

            LogoutCommand = new ViewModelCommand(ExecuteLogoutCommand);
            CreatRoomCommand = new ViewModelCommand(ExecuteCreatRoomCommand);
            LoadChatRoomsCommand = new ViewModelCommand(ExecuteLoadChatRoomsCommand);

            OpenLeftBoarMenuCommand = new ViewModelCommand(ExecuteOpenLeftBoarMenuCommand);

            CurrentPage = new View.EmptyChatView();

            LoadChatRoomsCommand.Execute(null);
            FilteredChatRooms = ChatRooms;
        }

        private void ExecuteOpenLeftBoarMenuCommand(object? obj)
        {
            LeftBorderMenuView page = new();
            LeftBorderMenuViewModel model = new();

            page.DataContext = model;

            Services.NavigationService.TopFrame.Content = page;

        }

        private async void ExecuteLoadChatRoomsCommand(object? obj)
        {
            var request = new GetChatRoomsDTO()
            {
                ClientId = ClientDTO.Id
            };

            var session = NetworkSession.Session;
            await session.SendAsync(request, RequestType.GetChatRooms);
            var response = await session.ResponseAsync<ChatRoomDTO[]>();

            if (response != null)
            {
                ChatRooms = response;
            }
            else
            {
                MessageBox.Show("Не удалось загрузить комнаты");
            }
            await NetworkSession.Session.ListenAsync();
        }

        private void ExecuteCreatRoomCommand(object? obj)
        {
            var win = new View.CreateRoomView();
            var dataContext = new CreateRoomViewModel(ClientDTO);
            win.DataContext = dataContext;
            dataContext.CloseAction = () => win.Close();

            win.ShowDialog();
            LoadChatRoomsCommand.Execute(null);
        }

        private void ExecuteLogoutCommand(object? obj)
        {
            NavigationService.MainFrame.Content = new View.LoginView();
            NetworkSession.Dispose();

        }

        private async void OpenSelectedChatRoom()
        {
            if (SelectedChatRoom == null) return;

            var chatRoomView = new ChatView();
            var chatViewModel = new ChatViewModel(SelectedChatRoom, ClientDTO);
            chatRoomView.DataContext = chatViewModel;
            CurrentPage = chatRoomView;

            try
            {
                var session = NetworkSession.Session;
                await session.SendAsync(SelectedChatRoom, RequestType.UpdateCurrentRoom);
            }
            catch (Exception ex) { }

        }

        private void ChatRoomFilter()
        {
            if(string.IsNullOrWhiteSpace(SearchText))
                FilteredChatRooms = ChatRooms;
            else
            {
                FilteredChatRooms = ChatRooms.Where(x => x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToArray();
            }
        }
    }
}
