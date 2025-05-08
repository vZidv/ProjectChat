using ChatClient.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatClient.CustomControls;

namespace ChatClient.ViewModels
{
    public class CreateRoomViewModel : BaseViewModel
    {
        private string _roomName;
        private int _ownerId;


        public string RoomName
        {
            get { return _roomName; }
            set
            {
                _roomName = value;
                OnPropertyChanged(nameof(RoomName));
            }
        }
        public int OwnerId
        {
            get { return _ownerId; }
            set
            {
                _ownerId = value;
                OnPropertyChanged(nameof(OwnerId));
            }
        }

        //Commands
        public ICommand CreateRoomCommand { get; }
        public Action CloseAction { get; set; }

        public CreateRoomViewModel()
        {
            CreateRoomCommand = new ViewModelCommand(ExecuteCreateRoomCommand, CanExecuteCreateRoomCommand);
        }

        public CreateRoomViewModel(ClientLoginDTO clientLoginDTO) : this()
        {
            OwnerId = clientLoginDTO.Id;
        }

        private async void ExecuteCreateRoomCommand(object? obj)
        {
            var newRoom = new ChatRoomDTO
            {
                Name = RoomName,
                OwnerId = this.OwnerId
            };
            
            var networkService = new Services.NetworkService();
            await networkService.ConnectAsync();
            await networkService.SendAsync(newRoom);
            bool result = await networkService.ResponseAsync<bool>();

            if (result)
            {
                MessageBox.Show("Комната успешно создана");
                CloseAction?.Invoke();
            }
            else
            {
                MessageBox.Show("Не удалось создать комнату");
            }
        }

        private bool CanExecuteCreateRoomCommand(object? obj) => !string.IsNullOrEmpty(RoomName);

    }
}
