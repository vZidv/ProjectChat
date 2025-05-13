using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Handlers
{
    public class HandlerRequest
    {
        public string Type { get; set; }
        public string Data { get; set; }
        public HandlerRequest(string type, string data)
        {
            Type = type;
            Data = data;
        }
        public HandlerRequest()
        {
            Type = string.Empty;
            Data = string.Empty;
        }
    }
}
