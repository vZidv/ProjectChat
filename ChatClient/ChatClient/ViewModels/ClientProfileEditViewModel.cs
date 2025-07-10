using ChatClient.CustomControls;
using ChatClient.Services;
using ChatShared.DTO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ChatClient.ViewModels
{
    public class ClientProfileEditViewModel : BaseViewModel
    {

        //Fields
        private ClientProfileDTO _clientProfileDTO;
        private BitmapImage? _avatarBitMap;
        private string? _avatarExtension;

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
                OnPropertyChanged(nameof(AvatarBitMap));
            }
        }
        //Commands
        public ICommand ClosePageCommand { get; }

        public ICommand ChooseAvatarCommand { get; }
        public ICommand UpdateCientProfileCommand { get; }

        public ClientProfileEditViewModel()
        {
            ClientProfileDTO = NetworkSession.ClientProfile;
            AvatarBitMap = AvatarService.Base64ToBitmapImage(NetworkSession.ClientProfile.AvatarBase64);

            ClosePageCommand = new ViewModelCommand(ExecuteClosePageCommand);
            ChooseAvatarCommand = new ViewModelCommand(ExecuteChooseAvatarCommand);
            UpdateCientProfileCommand = new ViewModelCommand(ExecuteUpdateCientProfileCommand, CanExecuteUpdateCientProfileCommand);
        }

        private bool CanExecuteUpdateCientProfileCommand(object? obj)
        {
            bool result = !string.IsNullOrWhiteSpace(ClientProfileDTO.Name) ||
                !string.IsNullOrWhiteSpace(ClientProfileDTO.LastName) ||
                !string.IsNullOrWhiteSpace(ClientProfileDTO.Email);

            if (!result)
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxType.Error);

            return result;
        }

        private void ExecuteUpdateCientProfileCommand(object? obj)
        {
            if (AvatarBitMap != null)
            {
                ClientProfileDTO.AvatarBase64 = AvatarService.BitmapImageToBase64(AvatarBitMap);
                ClientProfileDTO.AvatarExtension = _avatarExtension;
            }

            var session = NetworkSession.Session;
            session.SendAsync(ClientProfileDTO, ChatShared.DTO.Enums.RequestType.ClientProfileUpdate);

            NetworkSession.ClientProfile = ClientProfileDTO;
            ClosePageCommand.Execute(null);
        }

        private void ExecuteClosePageCommand(object? obj) => Services.NavigationService.TopFrame.GoBack();

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
                AvatarBitMap = new BitmapImage(new Uri(dialog.FileName));
            _avatarExtension = Path.GetExtension(dialog.FileName);
        }


    }
}
