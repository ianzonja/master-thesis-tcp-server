using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPserver
{
    class Room
    {
        public Player[] Players { get; set; }
        public string Id { get; set; }
        public string HostPlayfabId { get; set; }

    }
}
