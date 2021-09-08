using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPserver
{
    class DataManager
    {
        private readonly Object _Lock = new Object();

        public Dictionary<string, Player> GetAllPlayers()
        {
            lock (_Lock)
            {
                return Singleton.Instance.GetPlayers();
            }
        }

        public void SetAllPlayers(Dictionary<string, Player> players)
        {
            lock (_Lock)
            {
                Singleton.Instance.SetPlayers(players);
            }
        }

        public List<Room> GetAllRooms()
        {
            return Singleton.Instance.GetRooms();
        }

        public void SetAllRooms(List<Room> rooms)
        {
            Singleton.Instance.SetRooms(rooms);
        }
    }
}
