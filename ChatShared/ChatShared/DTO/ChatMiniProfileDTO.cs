using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class ChatMiniProfileDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool isGroup { get; set; }
        public string LastMessaget { get; set; }
        public DateTime LastActivity { get; set; }
    }
}
