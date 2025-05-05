using ChatClient.CustomControls;
using ChatClient.Services;
using ChatServer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatClient.ViewModels
{
    class LoginViewModel : BaseViewModel
    {
        //Fields
        private string _username;
        private string _password;
        private string _errorMessage;


        //Properties
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

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        //-> Commands
        public ICommand LoginCommand { get; }
        public ICommand SignUpCommand { get; }
        public ICommand RecoverPasswordCommand { get; }
        public ICommand ShowPasswordCommand { get; }
        public ICommand RememberPasswordCommand { get; }
        public ICommand TestCommand { get; }

        //Constructor
        public LoginViewModel()
        {
            LoginCommand = new ViewModelCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
            RecoverPasswordCommand = new ViewModelCommand(p => ExecuteRecoverPasswordCommand(""));
            SignUpCommand = new ViewModelCommand(p => ExecuteSignUpCommand());
            TestCommand = new ViewModelCommand(p => ExecuteTestCommand());
        }

        private void ExecuteTestCommand()
        {
            MessageBox.Show("Это прикол", "Это тайтал", MessageBoxButton.YesNo, MessageBoxType.Question);
        }

        private void ExecuteSignUpCommand()
        {
            var signUpView = new View.SignUpView();
            Services.NavigationService.MainFrame.Content = signUpView;
        }

        private bool CanExecuteLoginCommand(object? obj)
        {
            bool validData = true;
            if (string.IsNullOrWhiteSpace(Username) || Username.Length < 3 ||
               Password == null || Password.Length < 3)
            {
                validData = false;
            }

            return validData;
        }

        private async void ExecuteLoginCommand(object? obj)
        {
            var service = new NetworkService();
            await service.ConnectAsync();

            var client = new ClientLoginDTO()
            {
                Login = Username,
                PasswordHash = Password
            };

            await service.SendAsync<ClientLoginDTO>(client);
            bool response = await service.ResponseAsync<bool>();

            if (response)
            {
                var mainViewModel = new ViewModels.MainViewModel(client);
                var mainView = new View.MainView();
                mainView.DataContext = mainViewModel;
                Services.NavigationService.MainFrame.Content = mainView;
            }
        }

        private void ExecuteRecoverPasswordCommand(string username)
        {
            throw new NotImplementedException();
        }

    }
}
