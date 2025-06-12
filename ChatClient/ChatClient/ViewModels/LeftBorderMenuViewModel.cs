using ChatClient.CustomControls;
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

        //Property

        //Commands

        public ICommand ClosePageCommand { get;  }

        public ICommand OpenProjectLinkCommand { get; }
        public ICommand OpenProjectVersionLinkCommand { get; }
        public ICommand OpenAboutProgramCommand { get; }

        public LeftBorderMenuViewModel()
        {
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
