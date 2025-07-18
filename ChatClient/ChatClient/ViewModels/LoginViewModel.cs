﻿using ChatClient.CustomControls;
using ChatClient.Services;
using ChatShared.DTO;
using ChatShared.DTO.Enums;
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

        //Constructor
        public LoginViewModel()
        {
            LoginCommand = new ViewModelCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
            RecoverPasswordCommand = new ViewModelCommand(p => ExecuteRecoverPasswordCommand(""));
            SignUpCommand = new ViewModelCommand(p => ExecuteSignUpCommand());

            ApplyTheme();
        }

        private void ApplyTheme()
        {
            NetworkSession.Settings = new SettingsService();
            SettingsDTO settings = NetworkSession.Settings.GetSettingsAsync();

            Uri url = new Uri($@"Styles\{settings.Theme}Theme.xaml", UriKind.Relative);
            var app = (App)System.Windows.Application.Current;
            app.ChangeTheme(url);
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
            NetworkSession.Session = new NetworkService(App.EventAggregator);
            await NetworkSession.Session.ConnectAsync();

            var session = NetworkSession.Session;

            var client = new ClientLoginDTO()
            {
                Login = Username,
                PasswordHash = Password
            };

            await session.SendAsync<ClientLoginDTO>(client, RequestType.Login);
            LoginResultDTO response = await session.ResponseAsync<LoginResultDTO>();

            if (response.Success)
            {
                NetworkSession.ClientProfile = response.ClientProfileDTO;
                NetworkSession.Token = response.Token;

                var mainView = new View.MainView();

                Services.NavigationService.MainFrame.Content = mainView;
            }
            else
            {
                MessageBox.Show(response.ErrorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxType.Error);
            }
        }

        private void ExecuteRecoverPasswordCommand(string username)
        {
            throw new NotImplementedException();
        }

    }
}
