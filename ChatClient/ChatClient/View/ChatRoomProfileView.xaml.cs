﻿using System;
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
    /// Interaction logic for ChatRoomProfileView.xaml
    /// </summary>
    public partial class ChatRoomProfileView : Page
    {
        public ChatRoomProfileView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) => (sender as Button).ContextMenu.IsOpen = true;

    }
}
