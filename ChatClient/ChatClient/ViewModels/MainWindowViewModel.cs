using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ChatClient.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        private Page _currentPage;

        public Page CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }
        public MainWindowViewModel()
        {     
           CurrentPage = new View.LoginView();
        }

    }
   
}
