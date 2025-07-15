using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class UpdateClientSettingsDTO
    {
        public int ClientId {  get; set; }
        public SettingsDTO Settings { get; set; }
    }
}
