using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class AddContactDTO
    {
        public int SenderClientId { get; set; }
        public int ReceiverClientId { get; set; }
    }
}
