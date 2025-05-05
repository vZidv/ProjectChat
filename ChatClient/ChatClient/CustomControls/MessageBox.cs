using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;

namespace ChatClient.CustomControls
{
    public static partial class MessageBox
    {
        public static void Show(string message)
        {
            var msView = new MessageBoxView();
            var msViewModel = new MessageBoxViewModel("Внимание", message, MessageBoxButton.OK, MessageBoxType.Information);

            msView.DataContext = msViewModel;

            SystemSounds.Exclamation.Play();
            msView.ShowDialog();
        }

        public static void Show(string message, string title)
        {
            var msView = new MessageBoxView();
            var msViewModel = new MessageBoxViewModel(title, message, MessageBoxButton.OK, MessageBoxType.Information);

            msView.DataContext = msViewModel;

            SystemSounds.Exclamation.Play();
            msView.ShowDialog();
        }

        public static bool Show(string message, string title, MessageBoxButton msButton, MessageBoxType msType)
        {
            var msView = new MessageBoxView();
            var msViewModel = new MessageBoxViewModel(title, message, msButton, msType);

            msView.DataContext = msViewModel;
            msViewModel.RequestClose += () => { msView.Close(); };

            SystemSounds.Exclamation.Play();
            msView.ShowDialog();

            return msViewModel.Result;
        }
    }
}
