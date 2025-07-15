using ChatClient.Services;
using ChatShared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChatClient.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        //Fields 
        private SettingsDTO _settingsDTO { get; set; }

        //Property
        public SettingsDTO SettingsDTO
        {
            get { return _settingsDTO; }

            set
            {
                _settingsDTO = value;
                OnPropertyChanged(nameof(SettingsDTO));
            }
        }

        //Command
        public ICommand ClosePageCommand { get; }
        public ICommand ApplyThemeCommand { get; }

        public SettingsViewModel()
        {
            ClosePageCommand = new ViewModelCommand(ExecuteClosePageCommand);
            ApplyThemeCommand = new ViewModelCommand(ExecuteApplyThemeCommand);

            SettingsDTO = NetworkSession.Settings.GetSettingsAsync();
        }

        private void ExecuteClosePageCommand(object? obj)
        {
            Services.NavigationService.TopFrame.Content = null;
            NetworkSession.Settings.SaveSettingAsync(SettingsDTO);
        }

        private void ExecuteApplyThemeCommand(object? obj) 
        {
            Uri url = new Uri($@"Styles\{_settingsDTO.Theme}Theme.xaml", UriKind.Relative);
            var app = (App)Application.Current;
            app.ChangeTheme(url);
        }

    }
}
