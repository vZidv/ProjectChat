using ChatClient.CustomControls;
using ChatClient.Services;
using ChatClient.View;
using ChatShared.DTO;
using ChatShared.Events;
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
    public class EditRoomViewModel : BaseViewModel
    {
        //Fields
        private ChatMiniProfileDTO _chatDTO;
        private BitmapImage? _avatarBitMap;
        private string? _avatarExtension;

        //Property
        public ChatMiniProfileDTO ChatDTO
        {
            get { return _chatDTO; }
            set
            {
                _chatDTO = value;
                OnPropertyChanged(nameof(ChatDTO));
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
        public ICommand UpdateRoomProfileCommand { get; }

        public EditRoomViewModel() { }

        public EditRoomViewModel(ChatMiniProfileDTO chatMiniProfileDTO)
        {
            ChatDTO = chatMiniProfileDTO;
            AvatarBitMap = AvatarService.Base64ToBitmapImage(ChatDTO.AvatarBase64);

            ClosePageCommand = new ViewModelCommand(ExecuteClosePageCommand);

            ChooseAvatarCommand = new ViewModelCommand(ExecuteChooseAvatarCommand);
            UpdateRoomProfileCommand = new ViewModelCommand(ExecuteUpdateRoomProfileCommand, CanExecuteUpdateRoomProfileCommand);
        }

        private bool CanExecuteUpdateRoomProfileCommand(object? obj)
        {
            return string.IsNullOrWhiteSpace(ChatDTO.Name) ? false : true;
        }

        private void ExecuteUpdateRoomProfileCommand(object? obj)
        {
            if (AvatarBitMap != null)
            {
                ChatDTO.AvatarBase64 = AvatarService.BitmapImageToBase64(AvatarBitMap);
                ChatDTO.AvatarExtension = _avatarExtension;
            }

            var session = NetworkSession.Session;
            session.SendAsync(ChatDTO, ChatShared.DTO.Enums.RequestType.UpdateRoomProfile);

            App.EventAggregator.Publish(new UpdateChatRoomProfileEvent(ChatDTO));
            NavigationService.TopFrame.Content = new ChatRoomProfileView() { DataContext = new ChatRoomProfileViewModel(ChatDTO) };
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
