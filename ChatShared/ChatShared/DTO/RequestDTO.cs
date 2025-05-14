using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class RequestDTO <T>
    {
        public string Token { get; set; } = string.Empty;
        public RequestType Type { get; set; }
        public T? Data { get; set; }
    }
}
