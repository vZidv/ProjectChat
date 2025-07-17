using ChatClient.CustomControls;
using ChatClient.Services;
using ChatShared.DTO;
using ChatShared.DTO.Enums;
using ChatShared.Events;
using ControlzEx.Standard;
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
    class CreatRoomViewModel : BaseViewModel
    {
        //Fields
        private string _roomName;

        private BitmapImage? _avatarBitMap;
        private string? _avatarExtension;

        //Property

        public string RoomName
        {
            get => _roomName;
            set
            {
                _roomName = value;
                OnPropertyChanged(nameof(RoomName));
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

        public ICommand CreateRoomCommand { get; }
        public ICommand ChooseAvatarCommand { get; }

        public CreatRoomViewModel()
        {
            ClosePageCommand = new ViewModelCommand(ExecuteClosePageCommand);

            ChooseAvatarCommand = new ViewModelCommand(ExecuteChooseAvatarCommand);
            CreateRoomCommand = new ViewModelCommand(ExecuteCreateRoomCommand, CanExecuteCreateRoomCommand);

            App.EventAggregator.Subscribe<CreatRoomEvent>(onCreatRoomCheck);
        }

        private async void onCreatRoomCheck(CreatRoomEvent @event)
        {
            CreatChatRoomResultDTO result = @event.CreatChatRoomResultDTO;
            if (!result.Success)
            {
                MessageBox.Show($"Не удалось создать комнату: {result.ErrorMessage}", "Ошибка создания комнаты", MessageBoxButton.OK, MessageBoxType.Error);
                return;
            }

            await AddClientInChatGroupCommand(@event.CreatChatRoomResultDTO.ChatRoomDTO, NetworkSession.ClientProfile.Id);
            NavigationService.TopFrame.Content = null;
        }

        private async Task AddClientInChatGroupCommand(ChatRoomDTO chatRoomDTO, int clientId)
        {
            var request = new JoinInChatRoomDTO()
            {
                ChatRoomId = chatRoomDTO.Id,
                ClientId = clientId
            };

            var session = NetworkSession.Session;
            await session.SendAsync(request, RequestType.JoimInChatGroup);
        }

        private bool CanExecuteCreateRoomCommand(object? obj)
        {
            return String.IsNullOrWhiteSpace(RoomName) ? false : true;
        }

        private void ExecuteCreateRoomCommand(object? obj)
        {

            ChatRoomDTO newRoom = new()
            {
                Name = RoomName,
                ChatType = ChatType.Group,
                OwnerId = NetworkSession.ClientProfile.Id,
                IsPrivate = false,
                AvotarBase64 = AvatarService.BitmapImageToBase64(AvatarBitMap),
                AvatarExtension = _avatarExtension
            };

            NetworkSession.Session.SendAsync(newRoom, RequestType.CreatRoom);
        }

        private void ExecuteClosePageCommand(object? obj) => Services.NavigationService.TopFrame.Content = null;

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
