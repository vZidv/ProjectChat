using ChatClient.Services;
using ChatClient.View;
using ChatShared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ChatClient.ViewModels
{
    class ChatRoomProfileViewModel : BaseViewModel
    {
        //Fields
        private ChatMiniProfileDTO _chatDTO;
        private BitmapImage? _avatarBitMap;

        private Visibility _editButtonVisibility = Visibility.Visible;

        //Properties

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
                OnPropertyChanged(nameof(_avatarBitMap));
            }
        }

        public Visibility EditButtonVisibility
        {
            get { return _editButtonVisibility; }
            set
            {
                _editButtonVisibility = value;
                OnPropertyChanged(nameof(EditButtonVisibility));
            }
        }

        //Commands
        public ICommand ClosePageCommand { get; }

        public ICommand OpenEditPageCommand { get; }

        public ChatRoomProfileViewModel() {}
        
        public ChatRoomProfileViewModel(ChatMiniProfileDTO chatMiniProfileDTO)
        {
            ChatDTO = chatMiniProfileDTO;
            AvatarBitMap = AvatarService.Base64ToBitmapImage(ChatDTO.AvatarBase64);

            ClosePageCommand = new ViewModelCommand(ExecuteClosePageCommand);
            OpenEditPageCommand = new ViewModelCommand(ExecuteOpenEditPageCommand, CanExecuteOpenEditPageCommand);
        }

        private bool CanExecuteOpenEditPageCommand(object? obj)
        {
            var result = ChatDTO.ChatType != ChatShared.DTO.Enums.ChatType.Private;
            if(!result)
                EditButtonVisibility = Visibility.Hidden;
            return result;
        }

        private void ExecuteOpenEditPageCommand(object? obj)
        {
            var view = new EditRoomView();
            view.DataContext = new EditRoomViewModel(ChatDTO);
            NavigationService.TopFrame.Content = view;
        }

        private void ExecuteClosePageCommand(object? obj) => Services.NavigationService.TopFrame.Content = null;
    }
}
