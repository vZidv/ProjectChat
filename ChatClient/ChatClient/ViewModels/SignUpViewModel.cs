using ChatClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatShared.DTO;
using ChatShared.DTO.Enums;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.IO;
using ChatClient.CustomControls;

namespace ChatClient.ViewModels
{
    public class SignUpViewModel : BaseViewModel
    {
        //Fields
        private string _login;
        private string _password;

        private string _email;
        private string _name;
        private string _lastName;

        private BitmapImage _avatarImage;
        private string _avatarExtension;

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
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }
        public BitmapImage AvatarImage
        {
            get { return _avatarImage; }
            set
            {
                _avatarImage = value;
                OnPropertyChanged(nameof(AvatarImage));
            }
        }

        //Commands
        public ICommand GoBackCommand { get; }
        public ICommand SignUpCommand { get; }

        public ICommand ChooseAvatarCommand { get; }

        public SignUpViewModel()
        {
            GoBackCommand = new ViewModelCommand(ExcuteGoBackCommand);
            SignUpCommand = new ViewModelCommand(ExecuteSignUpCommand, CanExecuteSignUpCommand);
            ChooseAvatarCommand = new ViewModelCommand(ExecuteChooseAvatarCommand);
        }

        private void ExecuteChooseAvatarCommand(object? obj)
        {
            OpenFileDialog dialog = new()
            {
                Title = "Выбор аватарки",
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*",
                Multiselect = false
            };

            Nullable<bool> result = dialog.ShowDialog();

            var maxSize = 200 * 1024;
            var size = new FileInfo(dialog.FileName).Length;

            if (size > maxSize)
            {
                MessageBox.Show("Размер файла не должен привышать 200 Кб!", "Ошибка", MessageBoxButton.OK, MessageBoxType.Error);
                return;
            }

            if (result.HasValue)
                AvatarImage = new BitmapImage(new Uri(dialog.FileName));
            _avatarExtension = Path.GetExtension(dialog.FileName);
        }

        private async void ExecuteSignUpCommand(object? obj)
        {
            var newClient = new ClientSignUpDTO
            {
                Login = Login,
                PasswordHash = Password,

                Email = Email,
            };
            if (!string.IsNullOrWhiteSpace(Name) || !string.IsNullOrWhiteSpace(LastName))
            {
                newClient.Name = Name;
                newClient.LastName = LastName;
            }
            if (AvatarImage != null)
            {
                newClient.AvatarBase64 = AvatarService.BitmapImageToBase64(AvatarImage);
                newClient.AvatarExtension = _avatarExtension;
            }

            if(NetworkSession.Session == null)
            {
                NetworkSession.Session = new NetworkService(App.EventAggregator);
                await NetworkSession.Session.ConnectAsync();
            }

            var session = NetworkSession.Session;
            await session.SendAsync<ClientSignUpDTO>(newClient, RequestType.Register);
            bool result = await session.ResponseAsync<bool>();

            if (result)
            {
                GoBackCommand.Execute(null);

                NetworkSession.Settings = new SettingsService();
                NetworkSession.Settings.CreatNewSettingFileAsync();
            }

        }


        private bool CanExecuteSignUpCommand(object? obj)
        {
            if (Login == null || Login.Length < 3 ||
                 Password == null || Password.Length < 3 ||
               Email == null || Email.Length < 5)
                return false;
            return true;
        }

        private void ExcuteGoBackCommand(object? obj) => Services.NavigationService.MainFrame.GoBack();



    }
}
