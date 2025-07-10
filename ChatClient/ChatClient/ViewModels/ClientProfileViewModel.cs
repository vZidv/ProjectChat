using ChatClient.Services;
using ChatShared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using MahApps.Metro.Controls;
using ChatClient.CustomControls;

using MessageBox = ChatClient.CustomControls.MessageBox;
using MessageBoxButton = ChatClient.CustomControls.MessageBoxButton;
using System.Windows.Media.Imaging;
using ChatClient.View;

namespace ChatClient.ViewModels
{
    public class ClientProfileViewModel : BaseViewModel
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

        public ICommand ClosePageCommand { get; }
        public ICommand CopyLoginToBufferCommand { get; }
        public ICommand OpenEditProfilePageCommand { get; }

        public ClientProfileViewModel() 
        {
            ClientProfileDTO = NetworkSession.ClientProfile;
            AvatarBitMap = AvatarService.Base64ToBitmapImage(NetworkSession.ClientProfile.AvatarBase64);

            ClosePageCommand = new ViewModelCommand(ExecuteClosePageCommand);
            CopyLoginToBufferCommand = new ViewModelCommand(ExecuteCopyLoginToBufferCommand);
            OpenEditProfilePageCommand = new ViewModelCommand(ExecuteOpenEditProfilePageCommand);
        }

        private void ExecuteOpenEditProfilePageCommand(object? obj)
        {
            var view = new ClientProfileEditView();
            view.DataContext = new ClientProfileEditViewModel();
            NavigationService.TopFrame.Content = view;
        }

        private void ExecuteCopyLoginToBufferCommand(object? obj)
        {
            Clipboard.SetText(ClientProfileDTO.Login);
            MessageBox.Show("Логин был скопирован в буфер обмена.", "Внимание", MessageBoxButton.OK, MessageBoxType.Information);
        }

        private void ExecuteClosePageCommand(object? obj) => Services.NavigationService.TopFrame.Content = null;
    }
}
