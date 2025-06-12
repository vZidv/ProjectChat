using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ChatClient.Services
{
    public static class NavigationService
    {
        public static Frame? TopFrame { get; set; }
        public static Frame? MainFrame { get; set; }
    }
}
