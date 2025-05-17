using ChatShared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.Events
{
    public abstract class ChatEvent
    {
        public ResponseType Type { get;}

        protected ChatEvent(ResponseType type) 
        {
            Type = type;
        }
    }
}
