using ChatShared.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using ChatClient.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatShared.DTO.Enums;
using ChatShared.Events;

namespace ChatClient.ViewModels
{
    class ClientContactsViewModel : BaseViewModel
    {
        //Fields
        private ChatMiniProfileDTO _selectedChat;

        private ObservableCollection<ChatMiniProfileDTO> _contacts;
        public ObservableCollection<ChatMiniProfileDTO> _localSearchResults;

        private string _searchText;
        //Property
        public ChatMiniProfileDTO SelectedChat
        {
            get
            {
                return _selectedChat;
            }
            set
            {
                _selectedChat = value;
                OnPropertyChanged(nameof(SelectedChat));
                OpenSelectedChatRoom();
            }
        }
        public ObservableCollection<ChatMiniProfileDTO> Contacts
        {
            get { return _contacts; }
            set
            {
                _contacts = value;
                OnPropertyChanged(nameof(Contacts));
            }
        }
        public ObservableCollection<ChatMiniProfileDTO> LocalSearchResults
        {
            get { return _localSearchResults; }
            set
            {
                _localSearchResults = value;
                OnPropertyChanged(nameof(LocalSearchResults));
            }
        }
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                SearchContactCommand.Execute(this);
            }
        }
        //Commands
        public ICommand ClosePageCommand { get; }
        public ICommand LoadContactsCommand { get; }

        public ICommand SearchContactCommand { get; }

        public ClientContactsViewModel()
        {
            ClosePageCommand = new ViewModelCommand(ExecuteClosePageCommand);
            LoadContactsCommand = new ViewModelCommand(ExecuteLoadContactsCommand);

            SearchContactCommand = new ViewModelCommand(ExecuteSearchContactCommand, CanExecuteSearchContactCommand);

            App.EventAggregator.Subscribe<GetContactsEvent>(onLoadContacts);

            LoadContactsCommand.Execute(null);
        }

        private bool CanExecuteSearchContactCommand(object? obj)
        {
            return !string.IsNullOrWhiteSpace(SearchText);
        }

        private void ExecuteSearchContactCommand(object? obj)
        {
            LocalSearchResults = new ObservableCollection<ChatMiniProfileDTO>(Contacts.Where(c => c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList());
        }

        private void ExecuteLoadContactsCommand(object? obj)
        {
            var request = new GetChatsDTO() { ClientId = NetworkSession.ClientProfile.Id };

            var session = NetworkSession.Session;
            session.SendAsync(request, RequestType.GetContacts);
        }

        private void ExecuteClosePageCommand(object? obj) => Services.NavigationService.TopFrame.Content = null;

        private void onLoadContacts(GetContactsEvent @event)
        {
            var data = @event.ChatMiniProfileDTOs;
            LocalSearchResults = Contacts = new ObservableCollection<ChatMiniProfileDTO>(data);

        }
    }
}
