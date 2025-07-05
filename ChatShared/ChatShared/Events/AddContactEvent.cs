using ChatShared.DTO;
using ChatShared.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.Events
{
    public class AddContactEvent : ChatEvent
    {
        public AddContactResultDTO AddContactResultDTO { get; set; }

        public AddContactEvent(AddContactResultDTO result) : base(ResponseType.AddContactResult)
        {
            AddContactResultDTO = result;
        }
    }
}
