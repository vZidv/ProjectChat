using ChatShared.DTO;
using ChatShared.Events;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Services
{
    public class SettingsService
    {

        private readonly string _fileName = "Settings.json";

        public SettingsService()
        {
        }

        public async Task<bool> SaveSettingAsync(SettingsDTO settingsDTO)
        {
            if (settingsDTO == null)
                return false;

            var json = JsonConvert.SerializeObject(settingsDTO);
            await File.WriteAllTextAsync(_fileName, json);

            return true;
        }

        public SettingsDTO GetSettingsAsync()
        {
            if (File.Exists(_fileName))
            {
                var json = File.ReadAllText(_fileName);
                return JsonConvert.DeserializeObject<SettingsDTO>(json);
            }

            return CreatNewSettingFileAsync();
        }


        public SettingsDTO CreatNewSettingFileAsync()
        {
            SettingsDTO settingsDTO = new()
            {
                Theme = "White",
                FontSize = 20
            };

            var json = JsonConvert.SerializeObject(settingsDTO);
            File.WriteAllTextAsync(_fileName, json);

            return settingsDTO;
        }
    }
}
