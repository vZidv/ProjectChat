using ChatClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ChatClient.CustomControls
{
    public class MessageBoxViewModel : ViewModels.BaseViewModel
    {
        //Properties
        private string _title;
        private string _message;

        private MessageBoxButton _msButtons;
        private MessageBoxType _msType;

        private string _icon;
        private Brush _iconColor;

        private bool _result;
        private Visibility _isOkVisible = Visibility.Hidden;
        private Visibility _isYesNoVisible = Visibility.Hidden;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public MessageBoxButton Buttons
        {
            get { return _msButtons; }
            set
            {
                _msButtons = value;
                OnPropertyChanged(nameof(Buttons));
            }
        }

        public MessageBoxType Type
        {
            get { return _msType; }
            set
            {
                _msType = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public string Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        public Brush IconColor
        {
            get { return _iconColor; }
            set
            {
                _iconColor = value;
                OnPropertyChanged(nameof(IconColor));
            }
        }

        public bool Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
            }
        }

        public Visibility IsOkVisible
        {
            get { return _isOkVisible; }
            set
            {
                _isOkVisible = value;
                OnPropertyChanged(nameof(IsOkVisible));
            }
        }
        public Visibility IsYesNoVisible
        {
            get { return _isYesNoVisible; }
            set
            {
                _isYesNoVisible = value;
                OnPropertyChanged(nameof(IsYesNoVisible));
            }
        }

        //Commands
        public ICommand OkCommand { get; }
        public ICommand YesCommand { get; }
        public ICommand NoCommand { get; }
        public event Action RequestClose;

        public MessageBoxViewModel(string title, string message, MessageBoxButton msButtons, MessageBoxType msType)
        {
            Title = title;
            Message = message;

            _msButtons = msButtons;
            _msType = msType;

            SetIcon();
            SetButtons();

            OkCommand = new ViewModelCommand(ExecuteOkCommand, CanExecuteOkCommand);
            YesCommand = new ViewModelCommand(ExecuteYesCommand, CanExecuteYesCommand);
            NoCommand = new ViewModelCommand(ExecuteNoCommand, CanExecuteNoCommand);
        }

        private bool CanExecuteNoCommand(object? obj) => Buttons == MessageBoxButton.YesNo;
        private bool CanExecuteYesCommand(object? obj) => Buttons == MessageBoxButton.YesNo;
        private bool CanExecuteOkCommand(object? obj) => Buttons == MessageBoxButton.OK;


        private void ExecuteNoCommand(object? obj)
        {
            Result = false;
            RequestClose?.Invoke();
        }

        private void ExecuteYesCommand(object? obj)
        {
            Result = true;
            RequestClose?.Invoke();
        }


        private void ExecuteOkCommand(object? obj)
        {
            Result = true;
            RequestClose?.Invoke();
        }

        public void SetIcon()
        {
            switch (Type)
            {
                case MessageBoxType.Error:
                    Icon = "\uEB90";
                    IconColor = Brushes.Red;
                    break;
                case MessageBoxType.Information:
                    Icon = "\uF167";
                    IconColor = Brushes.DodgerBlue;
                    break;
                case MessageBoxType.Question:
                    Icon = "\uE9CE";
                    IconColor = Brushes.DodgerBlue;
                    break;
            }
        }

        public void SetButtons()
        {
            switch (Buttons)
            {
                case MessageBoxButton.OK:
                    IsOkVisible = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNo:
                    IsYesNoVisible = Visibility.Visible;
                    break;
            }
        }
    }
}
