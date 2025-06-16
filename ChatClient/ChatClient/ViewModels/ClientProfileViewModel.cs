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

namespace ChatClient.ViewModels
{
    public class ClientProfileViewModel : BaseViewModel
    {
        //Fields
        private ClientProfileDTO _clientProfileDTO;
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
        //Commands

        public ICommand ClosePageCommand { get; }
        public ICommand CopyLoginToBufferCommand { get; }

        public ClientProfileViewModel() 
        {
            ClientProfileDTO = NetworkSession.ClientProfile;

            ClosePageCommand = new ViewModelCommand(ExecuteClosePageCommand);
            CopyLoginToBufferCommand = new ViewModelCommand(ExecuteCopyLoginToBufferCommand);
        }

        private void ExecuteCopyLoginToBufferCommand(object? obj)
        {
            Clipboard.SetText(ClientProfileDTO.Login);
            MessageBox.Show("Логин был скопирован в буфер обмена.", "Внимание", MessageBoxButton.OK, MessageBoxType.Information);
        }

        private void ExecuteClosePageCommand(object? obj) => Services.NavigationService.TopFrame.Content = null;
    }
}
