using ChatClient.CustomControls;
using ChatClient.Services;
using ChatShared.DTO;
using ChatShared.DTO.Enums;
using ChatShared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatClient.ViewModels
{
    class CreatRoomViewModel : BaseViewModel
    {
        //Fields

        private string _roomName;

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
        //Commands

        public ICommand ClosePageCommand { get; }
        public ICommand CreateRoomCommand { get; }

        public CreatRoomViewModel()
        {
            ClosePageCommand = new ViewModelCommand(ExecuteClosePageCommand);
            CreateRoomCommand = new ViewModelCommand(ExecuteCreateRoomCommand, CanExecuteCreateRoomCommand);

            App.EventAggregator.Subscribe<CreatRoomEvent>(onCreatRoomCheck);
        }

        private void onCreatRoomCheck(CreatRoomEvent @event)
        {
            CreatChatRoomResultDTO result = @event.CreatChatRoomResultDTO;
            if (!result.Success)
            {
                MessageBox.Show($"Не удалось создать комнату: {result.ErrorMessage}", "Ошибка создания комнаты", MessageBoxButton.OK, MessageBoxType.Error);
                return;
            }

            NavigationService.TopFrame.Content = null;
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
                OwnerId = NetworkSession.ClientProfile.Id
            };

            NetworkSession.Session.SendAsync(newRoom, RequestType.CreatRoom);
        }

        private void ExecuteClosePageCommand(object? obj) => Services.NavigationService.TopFrame.Content = null;
    }
}
