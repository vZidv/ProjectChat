using ChatShared.Events;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IEventAggregator EventAggregator { get;} = new EventAggregator();
    }

}
