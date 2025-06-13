using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    internal class ClientProfileDTO
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
    }
}
