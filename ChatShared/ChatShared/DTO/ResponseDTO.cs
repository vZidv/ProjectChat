using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class ResponseDTO<T>
    {
        public ResponseType Type { get; set; }
        public T Data { get; set; }
        public string? Message { get; set; }
    }
}
