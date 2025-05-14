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
    public class SignUpViewModel : BaseViewModel
    {
        //Fields
        private string _login;
        private string _password;
        private string email;

        //Properties
        public string Login
        {
            get { return _login; }
            set
            {
                _login = value;
                OnPropertyChanged(nameof(Login));
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        //Commands
        public ICommand GoBackCommand { get; }
        public ICommand SignUpCommand { get; }

        public SignUpViewModel()
        {
            GoBackCommand = new ViewModelCommand(ExcuteGoBackCommand);
            SignUpCommand = new ViewModelCommand(ExecuteSignUpCommand, CanExecuteSignUpCommand);
        }

        private async void ExecuteSignUpCommand(object? obj)
        {
            var newClient = new ClientSignUpDTO
            {
                Login = Login,
                PasswordHash = Password,
                Email = Email
            };
            var session = NetworkSession.Session;
            await session.SendAsync<ClientSignUpDTO>(newClient, ChatShared.DTO.RequestType.Register);
            bool result = await session.ResponseAsync<bool>();
            
            if (result)
                GoBackCommand.Execute(null);

        }

        private bool CanExecuteSignUpCommand(object? obj)
        {
           if(Login == null || Login.Length < 3 ||
                Password == null || Password.Length < 3 ||
              Email == null || Email.Length < 5)
                return false;
            return true;
        }

        private void ExcuteGoBackCommand(object? obj) => Services.NavigationService.MainFrame.GoBack();
        


    }
}
