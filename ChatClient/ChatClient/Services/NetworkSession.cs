﻿using ChatShared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatClient.Services
{
    public static class NetworkSession
    {
        public static ClientProfileDTO? ClientProfile { get; set; } = null!;
        public static NetworkService? Session { get; set; } = null!;
        public static string? Token { get; set; } = null!;
        public static SettingsService? Settings { get; set; }

        public static void Dispose()
        {
            if (Session != null)
                Session.Dispose();

            ClientProfile = null;
            Session = null;
        }
    }
}
