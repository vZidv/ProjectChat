using ChatShared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Services
{
    public static class NetworkSession
    {
        public static ClientLoginDTO? Client { get; set; }
        public static NetworkService? Session { get; set; }
        public static string? Token { get; set; }

        public static void Dispose()
        {
            if (Session != null)
                Session.Dispose();

            Client = null;
            Session = null;
        }
    }
}
