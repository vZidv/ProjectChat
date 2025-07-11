using ChatClient.ViewModels;
using ChatShared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatClient.View
{
    /// <summary>
    /// Interaction logic for ClientContactsView.xaml
    /// </summary>
    public partial class ClientContactsView : Page
    {
        public ClientContactsView()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ClientContactsViewModel vm)
                vm.SelectedChat = (sender as ListBox).SelectedItem as ChatMiniProfileDTO;
        }
    }
}
