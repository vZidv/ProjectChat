using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatShared.DTO
{
    public class GetRoomHistoryDTO
    {
        public int RoomId { get; set; }
        public int Limit { get; set; }
    }
}
