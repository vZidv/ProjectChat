using ChatClient.CustomControls;
using ChatClient.Services;
using ChatClient.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatClient.ViewModels
{
    class LeftBorderMenuViewModel : BaseViewModel
    {
        //Fields
        private string _clientName;
        //Property

        public string ClientName
        {
            get => _clientName;
            set
            {
                _clientName = value;
                OnPropertyChanged(nameof(ClientName));
            }
        }
        //Commands

        public ICommand ClosePageCommand { get;  }

        public ICommand OpenProjectLinkCommand { get; }
        public ICommand OpenProjectVersionLinkCommand { get; }
        public ICommand OpenAboutProgramCommand { get; }

        public LeftBorderMenuViewModel()
        {
            ClientName = NetworkSession.ClientProfile.Login;

            ClosePageCommand = new ViewModelCommand(ExecuteClosePageCommand);
            OpenProjectLinkCommand = new ViewModelCommand(ExecuteOpenProjectLinkCommand);
            OpenProjectVersionLinkCommand = new ViewModelCommand(ExecuteOpenProjectVersionLinkCommand);
            OpenAboutProgramCommand = new ViewModelCommand(ExecuteOpenAboutProgramCommand);

        }

        private void ExecuteOpenAboutProgramCommand(object? obj)
        {
            AboutProgramView view = new();
            AboutProgramViewModel viewModel = new();
            view.DataContext = viewModel;

            Services.NavigationService.TopFrame.Content = view;
        }

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
