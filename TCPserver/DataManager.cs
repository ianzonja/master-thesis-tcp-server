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
            lock (_Lock)
            {
                return Singleton.Instance.GetRooms();
            }
        }

        public void SetAllRooms(List<Room> rooms)
        {
            lock (_Lock)
            {
                Singleton.Instance.SetRooms(rooms);
            }
        }

        public void AddPlayerToRoom(Player player, string roomId)
        {
            lock (_Lock)
            {
                var rooms = Singleton.Instance.GetRooms();
                int roomIndex = -1;
                for (int i = 0; i < rooms.Count; i++)
                {
                    roomIndex = i;
                }
                if (roomIndex != -1)
                {
                    Player[] players = new Player[rooms.ElementAt(roomIndex).Players.Length + 1];
                    for (int i = 0; i < rooms.ElementAt(roomIndex).Players.Length; i++)
                    {
                        players[i] = rooms.ElementAt(roomIndex).Players[i];
                    }
                    players[rooms.ElementAt(roomIndex).Players.Length] = player;
                    rooms.ElementAt(roomIndex).Players = players;
                    Singleton.Instance.SetRooms(rooms);
                }
            }
        }

        public void UpdatePlayerIngameStatus(string playfabId, string roomId, string status)
        {
            var players = Singleton.Instance.GetPlayers();
            Player[] roomPlayers = null;
            var player = players[playfabId];
            player.InGameStatus = status;
            players[playfabId] = player;
            Room room = null;
            var rooms = Singleton.Instance.GetRooms();
            for(int i=0; i<rooms.Count; i++)
            {
                for(int j=0; j<rooms[i].Players.Length; j++)
                {
                    if(rooms[i].Players[j].PlayFabId == playfabId)
                    {
                        room = rooms[i];
                        room.Players[j] = player;
                        rooms[i] = room;
                    }
                }
            }
            this.SetAllRooms(rooms);
            this.SetAllPlayers(players);
        }

        public string SetPlayersStatusIngame(string playfabId, string roomId)
        {
            var players = Singleton.Instance.GetPlayers();
            Player[] roomPlayers = new Player[4];
            Room room = null;
            var rooms = Singleton.Instance.GetRooms();
            var playersReady = true;
            for (int i = 0; i < rooms.Count; i++)
            {
                if(rooms[i].HostPlayfabId == playfabId)
                {
                    for(int j=1; j <rooms[i].Players.Length; j++)
                    {
                        if (rooms[i].Players[j].InGameStatus != "READY")
                            playersReady = false;
                    }
                    if (playersReady){
                        for (int j = 0; j < rooms[i].Players.Length; j++)
                        {
                            var player = rooms[i].Players[j];
                            player.InGameStatus = "INGAME";
                            roomPlayers[j] = player;
                            players[player.PlayFabId] = player;
                        }
                        rooms[i].Players = roomPlayers;
                    }
                }
            }
            if (playersReady)
            {
                this.SetAllPlayers(players);
                this.SetAllRooms(rooms);
                return "OK";
            }
            return "FORBIDDEN";
        }
        public Room GetOneRoom(string roomId)
        {
            lock (_Lock)
            {
                var rooms = Singleton.Instance.GetRooms();
                Room room = null;
                for (int i = 0; i < rooms.Count; i++)
                {
                    if (rooms.ElementAt(i).Id == roomId)
                        room = rooms.ElementAt(i);
                }
                return room;
            }
        }

        public Player GetOnePlayer(string playfabId)
        {
            lock (_Lock)
            {
                var players = Singleton.Instance.GetPlayers();
                return players[playfabId];
            }
        }

        public string RemovePlayerFromRoom(string roomID, string playfabId, string kickedPlayerId)
        {
            lock (_Lock)
            {
                var room = this.GetOneRoom(roomID);
                var rooms = this.GetAllRooms();
                var players = this.GetAllPlayers();
                var player = players[kickedPlayerId];
                if (playfabId == room.HostPlayfabId)
                {
                    var roomPlayers = room.Players;
                    int index = -1;
                    for (int i = 0; i < roomPlayers.Length; i++)
                    {
                        if (roomPlayers[i].PlayFabId == kickedPlayerId)
                            index = i;
                    }
                    List<Player> tmpPlayers = new List<Player>(roomPlayers);
                    tmpPlayers.RemoveAt(index);
                    roomPlayers = tmpPlayers.ToArray();
                    room.Players = roomPlayers;
                    for (int i = 0; i < rooms.Count; i++)
                    {
                        if (rooms[i].Id == room.Id)
                            rooms[i] = room;
                    }
                    player.InGameStatus = "LOBBY";
                    players[player.PlayFabId] = player;
                    this.SetAllRooms(rooms);
                    this.SetAllPlayers(players);
                    return "OK";
                }
                else
                    return "FORBIDDEN";
            }
        }

        public Dictionary<string, Game> GetAllGames()
        {
            lock (_Lock)
            {
                return Singleton.Instance.GetGames();
            }
        }

        public void SetAllGames(Dictionary<string, Game> games)
        {
            lock (_Lock)
            {
                Singleton.Instance.SetGames(games);
            }
        }

        public Game GetOneGame(string gameId)
        {
            lock (_Lock)
            {
                return Singleton.Instance.GetOneGame(gameId);
            }
        }

        public void AddOneGameToGames(Game game)
        {
            lock (_Lock)
            {
                var games = Singleton.Instance.GetGames();
                if (games == null)
                    games = new Dictionary<string, Game>();
                games.Add(game.GameId.ToString(), game);
                Singleton.Instance.SetGames(games);
            }
        }

        public void SetAllPlayersIngameStatus(Room room)
        {
            var players = this.GetAllPlayers();
            foreach(var player in room.Players)
            {
                players[player.PlayFabId].InGameStatus = "GAME";
            }
            Singleton.Instance.SetPlayers(players);
        }

        public void RemoveRoom(Room room)
        {
            int index = -1;
            var rooms = this.GetAllRooms();
            for(int i=0; i<rooms.Count; i++)
            {
                if (rooms[i].Id == room.Id)
                    index = i;
            }
            if (index != -1)
                rooms.RemoveAt(index);
            Singleton.Instance.SetRooms(rooms);
        }
    }
}
