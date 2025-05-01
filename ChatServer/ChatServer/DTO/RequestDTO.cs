using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.DTO
{
    public class RequestDTO
    {
        public RequestType Type { get; set; }
        public object Data { get; set; }
    }
}
