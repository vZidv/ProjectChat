using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    /// Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView : Page
    {
        private INotifyCollectionChanged? _lastCollection;

        public ChatView()
        {
            InitializeComponent();
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ChatClient.ViewModels.ChatViewModel vm)
            {
                if (_lastCollection != null)
                    _lastCollection.CollectionChanged -= ChatMessages_CollectionChanged;

                if (vm.ChatMessageDTOs is INotifyCollectionChanged notifyCollection)
                {
                    notifyCollection.CollectionChanged += ChatMessages_CollectionChanged;
                    _lastCollection = notifyCollection;
                }

                ScrollToLastMessage();
            }
        }

        private void ScrollToLastMessage()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (message_listBox.Items.Count > 0)
                {
                    message_listBox.ScrollIntoView(message_listBox.Items[message_listBox.Items.Count - 1]);
                }
            }));
        }

        private void ChatMessages_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var scrollViewer = GetScrollViewer(message_listBox);
                if (scrollViewer != null)
                {
                    bool isAtBottom = scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight - 10;
                    if (isAtBottom && message_listBox.Items.Count > 0)
                    {
                        message_listBox.ScrollIntoView(message_listBox.Items[message_listBox.Items.Count - 1]);
                    }
                }
            }));
        }

        private static ScrollViewer? GetScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer) return (ScrollViewer)depObj;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                var result = GetScrollViewer(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}
