using ChatShared.DTO;
using ChatShared.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.Events
{
    public class GetClientSettingsEvent : ChatEvent
    {
        public SettingsDTO SettingsDTO { get; set; }

        public GetClientSettingsEvent(SettingsDTO settingsDTO) : base(ResponseType.GetClientSettings)
        {
            SettingsDTO = settingsDTO;
        }
    }
}
