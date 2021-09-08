using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPserver
{
    class Game
    {
        public Player[] players = new Player[4];
        public string HostUsername { get; set; }
        public RoundPoint[] Scoreboard { get; set; }
    }
}
