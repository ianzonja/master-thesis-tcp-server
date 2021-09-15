using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPserver
{
    class Command
    {
        public string CommandId { get; set; }

        public string SessionTicket { get; set; }

        public string RoomID { get; set; }

        public string PlayfabId { get; set; }

        public string KickedPlayerId { get; set; }
    }
}
