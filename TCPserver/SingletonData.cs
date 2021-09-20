using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPserver
{
    public sealed class Singleton
    {
        Dictionary<string, Player> Players;
        List<Room> Rooms;
        Dictionary<string, Game> Games;
        int NumberOfPlayers;
        private static readonly Singleton instance = new Singleton();
        static Singleton() {
        } // Make sure it's truly lazy
        private Singleton() {
        } // Prevent instantiation outside

        internal Dictionary<string, Game> GetGames()
        {
            return this.Games;
        }

        internal void SetGames(Dictionary<string, Game> games)
        {
            this.Games = games;
        }

        internal Game GetOneGame(string gameId)
        {
            return this.Games[gameId];
        }

        internal void AddOneGameToGames(string id, Game game)
        {
            this.Games.Add(id, game);
        }

        internal Dictionary<string, Player> GetPlayers()
        {
            return this.Players;
        }

        internal void AddPlayerToConnectedPlayersDictionary(Player player)
        {
            this.Players.Add(player.PlayFabId, player);
        }

        public static Singleton Instance { get { return instance; } }

        internal void SetPlayers(Dictionary<string, Player> players)
        {
            this.Players = players;
        }

        internal List<Room> GetRooms()
        {
            return this.Rooms;
        }

        internal void SetRooms(List<Room> rooms)
        {
            this.Rooms = rooms;
        }

        internal void AddRoom(Room room)
        {
            this.Rooms.Add(room);
        }
     
    }
}