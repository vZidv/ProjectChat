using ChatClient.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace ChatClient.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        //Fields
        ClientLoginDTO _clientDTO; //DTO for the logged-in user

        ChatRoomDTO[] _chatRooms; // List of chat rooms

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

        //Commands
        public ICommand LogoutCommand { get; }
        public ICommand CreatRoomCommand { get; }
        public ICommand LoadChatRoomsCommand { get; }

        public MainViewModel()
        {

        }
        public MainViewModel(ClientLoginDTO clientDTO)
        {
            ClientDTO = clientDTO;

            LogoutCommand = new ViewModelCommand(ExecuteLogoutCommand);
            CreatRoomCommand = new ViewModelCommand(ExecuteCreatRoomCommand);
            LoadChatRoomsCommand = new ViewModelCommand(ExecuteLoadChatRoomsCommand);
        }

        private async void ExecuteLoadChatRoomsCommand(object? obj)
        {
            var request = new GetChatRoomsDTO()
            {
                ClientId = ClientDTO.Id
            };
            var json = JsonConvert.SerializeObject(request);
            var bites = Encoding.UTF8.GetBytes(json + "\n");

            var networkService = new Services.NetworkService();
            await networkService.ConnectAsync();
            await networkService.SendAsync(bites);
            var response = await networkService.ResponseAsync<ChatRoomDTO[]>();

            if (response != null)
            {
                ChatRooms = response;
            }
            else
            {
                MessageBox.Show("Не удалось загрузить комнаты");
            }
        }

        private void ExecuteCreatRoomCommand(object? obj)
        {
            var win = new View.CreateRoomView();
            var dataContext = new CreateRoomViewModel(ClientDTO);
            win.DataContext = dataContext;
            dataContext.CloseAction = () => win.Close();

            win.ShowDialog();
        }

        private void ExecuteLogoutCommand(object? obj)
        {
            Services.NavigationService.MainFrame.Content = new View.LoginView();
        }
    }
}
