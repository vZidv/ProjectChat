using ChatClient.CustomControls;
using ChatClient.Services;
using ChatClient.View;
using ChatShared.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization.DataContracts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ChatClient.ViewModels
{
    class LeftBorderMenuViewModel : BaseViewModel
    {
        //Fields
        private ClientProfileDTO _clientProfileDTO;

        private BitmapImage? _avatarBitMap;
        //Property

        public ClientProfileDTO ClientProfileDTO
        {
            get => _clientProfileDTO;
            set
            {
                _clientProfileDTO = value;
                OnPropertyChanged(nameof(_clientProfileDTO));
            }
        }

        public BitmapImage? AvatarBitMap
        {
            get => _avatarBitMap;
            set
            {
                _avatarBitMap = value;
                OnPropertyChanged(nameof(_avatarBitMap));
            }
        }
        //Commands

        public ICommand ClosePageCommand { get;  }

        public ICommand OpenCreatRoomPageCommand { get; }
        public ICommand OpenClietnProfilePageCommand { get; }
        public ICommand OpenContactsPageCommand { get; }
        public ICommand OpenSettingsPageCommand { get; }

        public ICommand OpenProjectLinkCommand { get; }
        public ICommand OpenProjectVersionLinkCommand { get; }
        public ICommand OpenAboutProgramCommand { get; }

        public LeftBorderMenuViewModel()
        {
            ClientProfileDTO = NetworkSession.ClientProfile;
            AvatarBitMap = AvatarService.Base64ToBitmapImage(NetworkSession.ClientProfile.AvatarBase64);

            ClosePageCommand = new ViewModelCommand(ExecuteClosePageCommand);

            OpenCreatRoomPageCommand = new ViewModelCommand(ExecuteOpenCreatRoomPageCommand);
            OpenClietnProfilePageCommand = new ViewModelCommand(ExecuteOpenClietnProfilePageCommand);
            OpenContactsPageCommand = new ViewModelCommand(ExecuteOpenContactsPageCommand);
            OpenSettingsPageCommand = new ViewModelCommand(ExecuteOpenSettingsPageCommand);

            OpenProjectLinkCommand = new ViewModelCommand(ExecuteOpenProjectLinkCommand);
            OpenProjectVersionLinkCommand = new ViewModelCommand(ExecuteOpenProjectVersionLinkCommand);
            OpenAboutProgramCommand = new ViewModelCommand(ExecuteOpenAboutProgramCommand);

        }

        private void ExecuteOpenContactsPageCommand(object? obj) => Services.NavigationService.TopFrame.Content = new ClientContactsView() { DataContext = new ClientContactsViewModel()};
        private void ExecuteOpenClietnProfilePageCommand(object? obj)  => Services.NavigationService.TopFrame.Content = new ClientProfileView() { DataContext = new ClientProfileViewModel()};
        private void ExecuteOpenCreatRoomPageCommand(object? obj) => Services.NavigationService.TopFrame.Content = new CreatRoomView() {  };
        private void ExecuteOpenAboutProgramCommand(object? obj) => Services.NavigationService.TopFrame.Content = new AboutProgramView() { DataContext = new AboutProgramViewModel()};
        private void ExecuteOpenSettingsPageCommand(object? obj) => Services.NavigationService.TopFrame.Content = new SettingsView() { DataContext = new SettingsViewModel()};


        private void ExecuteClosePageCommand(object? obj) => Services.NavigationService.TopFrame.Content = null;

        private void ExecuteOpenProjectLinkCommand(object? obj) => OpenUrl("https://github.com/vZidv/ProjectChat.git");

        private void ExecuteOpenProjectVersionLinkCommand(object? obj) => OpenUrl("https://github.com/vZidv/ProjectChat/releases");

        private void OpenUrl(string url)
        {
            try
            {
                ProcessStartInfo psi = new()
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex) { }
        }

    }
}
