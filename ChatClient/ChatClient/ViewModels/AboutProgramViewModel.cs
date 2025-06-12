using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatClient.ViewModels
{
    class AboutProgramViewModel
    {
        //Fields

        //Properties

        //Commands
        
        public ICommand ClosePageCommand { get; }

        //urlCommands

        public ICommand LicenseUrlCommand { get; }
        public ICommand GithubUrlCommand { get; }
        public ICommand ProfGithubUrlCommand { get; }
        //public ICommand ProfGithubUrlCommand { get; }
        //public ICommand ProfGithubUrlCommand { get; }

        public AboutProgramViewModel()
        {
            ClosePageCommand = new ViewModelCommand(ExecuteClosePageCommand);

            LicenseUrlCommand = new ViewModelCommand(ExecuteLicenseUrlCommand);
            GithubUrlCommand = new ViewModelCommand(ExecuteGithubUrlCommand);
            ProfGithubUrlCommand = new ViewModelCommand(ExecuteProfGithubUrlCommand);
        }

        private void ExecuteProfGithubUrlCommand(object? obj) => OpenUrl("https://github.com/vZidv");

        private void ExecuteGithubUrlCommand(object? obj) => OpenUrl("https://github.com/vZidv/ProjectChat.git");

        private void ExecuteLicenseUrlCommand(object? obj) => OpenUrl("https://github.com/vZidv/ProjectChat.git");


        private void ExecuteClosePageCommand(object? obj) => Services.NavigationService.TopFrame.Content = null;


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
