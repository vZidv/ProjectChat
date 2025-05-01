using ChatServer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatClient.ViewModels
{
    class MainViewModel : BaseViewModel
    {

        ClientLoginDTO _clientDTO;
        private string _username;

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
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        //Commands
        public ICommand LogoutCommand { get; }

        public MainViewModel(ClientLoginDTO clientDTO)
        {
            ClientDTO = clientDTO;
            Username = ClientDTO.Login;

            LogoutCommand = new ViewModelCommand(ExecuteLogoutCommand);
        }

        private void ExecuteLogoutCommand(object? obj)
        {
            Services.NavigationService.MainFrame.Content = new View.LoginView();
        }
    }
}
