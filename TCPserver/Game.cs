using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPserver
{
    class Game
    {
        public string RoomId { get; set; }
        public Guid GameId { get; set; }

        public Player[] Players = new Player[4];
        public string HostUsername { get; set; }
        public RoundPoint[] Scoreboard { get; set; }
        public string DealerPlayfabId { get; set; }
        public Message[] Chat { get; set; }

        public Card[] CollectedCards { get; set; }

        public Card[] ShuffledCards { get; set; }

        public string PlayerIdToPlay { get; set; }
        public string Status { get; set; }
    }
}
