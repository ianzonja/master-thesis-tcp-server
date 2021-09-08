using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TCPserver
{
    class Program
    {
        static void Main(string[] args)
        {
            DataManager dm = new DataManager();
            dm.SetAllPlayers(new Dictionary<string, Player>());
            dm.SetAllRooms(new List<Room>());
            TCPserver server = new TCPserver();
            server.PokreniListener();
        }
    }
}
