using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class RequestDTO <T>
    {
        public RequestType Type { get; set; }
        public T? Data { get; set; }
    }
}
