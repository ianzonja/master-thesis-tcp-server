using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPserver
{
    class ObradaPoruke
    {
        private byte[] bytePoruka;
        private string pomocnaPoruka;
        private string[] poruke = { };
        bool izadiIzPetlje;

        private string[] elementiPoruke;
        public ObradaPoruke(byte[] primljenaPoruka)
        {
            bytePoruka = primljenaPoruka;
        }

        //metoda koja prepoznaje da se radi o login poruci. Prije ove metode (i ostalih metoda takve prirode) treba dekriptirati poruku koju je korisnik kriptirao.

        public string PrepoznavanjePoruke()
        {
            string json = "";
            try
            {
                json = Encoding.ASCII.GetString(bytePoruka).Replace("\0", "");
                //json = json.Remove(0, 1);
                //json = json.Remove(json.Length - 1, 1);
                Console.Write(json);
                Command jsonObject = JsonConvert.DeserializeObject<Command>(json);
                if (jsonObject != null)
                {
                    if (jsonObject.CommandId == "YOIJUSTLOGGEDIN")
                    {
                        Console.WriteLine("eurekica");
                        ValidateClient client = new ValidateClient(jsonObject.SessionTicket);
                        if (client.Response != null)
                        {
                            if (client.Response.code == 200)
                            {
                                string token = new TokenManager().GenerateToken(client.Response.data.UserInfo.PlayFabId);
                                Console.WriteLine("koji k");
                                Console.WriteLine(client.Response.data.UserInfo.Username);
                                Player player = new Player();
                                player.Email = client.Response.data.UserInfo.PrivateInfo.Email;
                                player.Username = client.Response.data.UserInfo.Username;
                                player.PlayFabId = client.Response.data.UserInfo.PlayFabId;
                                player.InGameStatus = "LOBBY";
                                GetUserData data = new GetUserData(player.PlayFabId);
                                if (data.Response != null)
                                {
                                    player.Loses = data.Response.data.Data.Loses.Value;
                                    player.Wins = data.Response.data.Data.Wins.Value;
                                    player.Draws = data.Response.data.Data.Draws.Value;
                                }
                                Singleton.Instance.AddPlayerToConnectedPlayersDictionary(player);
                                JwtPlayer jwtPlayer = new JwtPlayer().SetPlayer(player);
                                jwtPlayer.Jwt = token;
                                json = "{\"ResponseId\":\"YO\", \"MyData\":" + JsonConvert.SerializeObject(jwtPlayer) + "}";
                                Console.WriteLine(json);
                            }
                        }
                    }
                    if (jsonObject.CommandId == "YOGIVEMEROOMINFO")
                    {
                        var token = new TokenManager().DecryptToken(jsonObject.Jwt);
                        if (token != null)
                        {
                            if (Singleton.Instance.GetPlayers().ContainsKey(token.Id)) ;
                            {
                                string array = "[";
                                for (int i = 0; i < Singleton.Instance.GetPlayers().Values.Count; i++)
                                {
                                    foreach (var item in Singleton.Instance.GetPlayers())
                                    {
                                        Console.WriteLine("item");
                                    }
                                    array += JsonConvert.SerializeObject(Singleton.Instance.GetPlayers()) + ",";
                                }
                                array = array.Remove(array.Length - 1);
                                array += "]";
                                DataManager dm = new DataManager();
                                var players = dm.GetAllPlayers();
                                var currentPlayer = players[token.Id];
                                string playersJson = JsonConvert.SerializeObject(players.Values, Formatting.Indented);
                                var rooms = dm.GetAllRooms();
                                string roomsJson = JsonConvert.SerializeObject(rooms);
                                json = "{\"NumberOfPlayers\":\"" + players.Values.Count + "\", \"Players\":" + playersJson + ", \"Rooms\": " + roomsJson + "}";
                                Console.WriteLine(json);
                            }
                        }
                    }
                    if (jsonObject.CommandId == "YOIJUSTREGISTERED")
                    {
                        ValidateClient client = new ValidateClient(jsonObject.SessionTicket);
                        if (client.Response != null)
                        {
                            var token = new TokenManager().GenerateToken(client.Response.data.UserInfo.PlayFabId);
                            BasicPlayerData pbd = new BasicPlayerData();
                            pbd.Draws = "0";
                            pbd.Loses = "0";
                            pbd.Wins = "0";
                            UpdatePlayerData updatePlayerData = new UpdatePlayerData(pbd, client.Response.data.UserInfo.PlayFabId);
                            Player player = new Player();
                            player.Email = client.Response.data.UserInfo.PrivateInfo.Email;
                            player.Username = client.Response.data.UserInfo.Username;
                            player.PlayFabId = client.Response.data.UserInfo.PlayFabId;
                            player.InGameStatus = "LOBBY";
                            GetUserData data = new GetUserData(player.PlayFabId);
                            if (data.Response != null)
                            {
                                player.Loses = data.Response.data.Data.Loses.Value;
                                player.Wins = data.Response.data.Data.Wins.Value;
                                player.Draws = data.Response.data.Data.Draws.Value;
                            }
                            Singleton.Instance.AddPlayerToConnectedPlayersDictionary(player);
                            JwtPlayer jwtPlayer = new JwtPlayer().SetPlayer(player);
                            jwtPlayer.Jwt = token;
                            json = "{\"ResponseId\":\"YO\", \"MyData\":" + JsonConvert.SerializeObject(jwtPlayer) + "}";
                        }
                        //ValidateClient client = new ValidateClient(jsonObject.SessionTicket);
                        //if (client.Response != null)
                        //{
                        //    BasicPlayerData pbd = new BasicPlayerData();
                        //    pbd.Draws = "0";
                        //    pbd.Loses = "0";
                        //    pbd.Wins = "0";
                        //    UpdatePlayerData updatePlayerData = new UpdatePlayerData(pbd, client.Response.data.UserInfo.PlayFabId);
                        //    Player player = new Player();
                        //    player.Email = client.Response.data.UserInfo.PrivateInfo.Email;
                        //    player.Username = client.Response.data.UserInfo.Username;
                        //    player.PlayFabId = client.Response.data.UserInfo.PlayFabId;
                        //    player.InGameStatus = "LOBBY";
                        //    GetUserData data = new GetUserData(player.PlayFabId);
                        //    if (data.Response != null)
                        //    {
                        //        player.Loses = data.Response.data.Data.Loses.Value;
                        //        player.Wins = data.Response.data.Data.Wins.Value;
                        //        player.Draws = data.Response.data.Data.Draws.Value;
                        //    }
                        //    Singleton.Instance.AddPlayerToConnectedPlayersDictionary(player);
                        //    json = "{\"ResponseId\":\"YO\", \"MyData\":" + JsonConvert.SerializeObject(player) + "}";
                    }
                    if (jsonObject.CommandId == "YOIWANNAHOST")
                    {
                        var token = new TokenManager().DecryptToken(jsonObject.Jwt);
                        if (token != null)
                        {
                            Room room = new Room();
                            room.HostPlayfabId = token.Id;
                            room.Id = Guid.NewGuid().ToString();
                            var players = new DataManager().GetAllPlayers();
                            Player[] roomPlayers = new Player[1];
                            var player = players[token.Id];
                            player.InGameStatus = "ROOM";
                            players[token.Id] = player;
                            roomPlayers[0] = players[token.Id];
                            room.Players = roomPlayers;
                            json = "{\"ResponseId\":\"YO\", \"MyData\":" + JsonConvert.SerializeObject(room) + "}";
                            Singleton.Instance.AddRoom(room);
                        }
                        //ValidateClient client = new ValidateClient(jsonObject.SessionTicket);
                        //if (client.Response != null)
                        //{
                        //    Room room = new Room();
                        //    room.HostPlayfabId = client.Response.data.UserInfo.PlayFabId;
                        //    room.Id = Guid.NewGuid().ToString();
                        //    var players = new DataManager().GetAllPlayers();
                        //    Player[] roomPlayers = new Player[1];
                        //    var player = players[client.Response.data.UserInfo.PlayFabId];
                        //    player.InGameStatus = "ROOM";
                        //    players[client.Response.data.UserInfo.PlayFabId] = player;
                        //    roomPlayers[0] = players[client.Response.data.UserInfo.PlayFabId];
                        //    room.Players = roomPlayers;
                        //    json = "{\"ResponseId\":\"YO\", \"MyData\":" + JsonConvert.SerializeObject(room) + "}";
                        //    Singleton.Instance.AddRoom(room);
                        //}
                    }
                    if (jsonObject.CommandId == "YOIWANNAJOIN")
                    {
                        var token = new TokenManager().DecryptToken(jsonObject.Jwt);
                        if (token != null)
                        {
                            DataManager dm = new DataManager();
                            var player = dm.GetOnePlayer(token.Id);
                            player.InGameStatus = "ROOM";
                            var players = dm.GetAllPlayers();
                            players[token.Id] = player;
                            var room = dm.GetOneRoom(jsonObject.RoomID);
                            if (player != null && room != null)
                            {
                                dm.AddPlayerToRoom(player, jsonObject.RoomID);
                                json = "{\"ResponseId\":\"YO\", \"MyData\":" + JsonConvert.SerializeObject(room) + "}";
                            }
                            else
                                Console.WriteLine("User ne postoji u dictionaryu. ne mogu se spojiti sobi");
                        }
                        //ValidateClient client = new ValidateClient(jsonObject.SessionTicket);
                        //if (client.Response != null)
                        //{
                        //    DataManager dm = new DataManager();
                        //    var player = dm.GetOnePlayer(client.Response.data.UserInfo.PlayFabId);
                        //    player.InGameStatus = "ROOM";
                        //    var players = dm.GetAllPlayers();
                        //    players[client.Response.data.UserInfo.PlayFabId] = player;
                        //    var room = dm.GetOneRoom(jsonObject.RoomID);
                        //    if (player != null && room != null)
                        //    {
                        //        dm.AddPlayerToRoom(player, jsonObject.RoomID);
                        //        json = "{\"ResponseId\":\"YO\", \"MyData\":" + JsonConvert.SerializeObject(room) + "}";
                        //    }
                        //    else
                        //        Console.WriteLine("User ne postoji u dictionaryu. ne mogu se spojiti sobi");
                        //}
                    }
                    if (jsonObject.CommandId == "YOIMINROOM")
                    {
                        var token = new TokenManager().DecryptToken(jsonObject.Jwt);
                        if (token != null)
                        {
                            DataManager dm = new DataManager();
                            var player = dm.GetOnePlayer(jsonObject.PlayfabId);
                            var room = dm.GetOneRoom(jsonObject.RoomID);
                            if (player != null && room != null)
                            {
                                json = "{\"ResponseId\":\"YO\", \"MyData\":" + JsonConvert.SerializeObject(room) + "}";
                            }
                            else
                                Console.WriteLine("User ne postoji u dictionaryu. ne mogu se spojiti sobi");
                        }
                        //if (jsonObject.PlayfabId != null)
                        //{
                        //    DataManager dm = new DataManager();
                        //    var player = dm.GetOnePlayer(jsonObject.PlayfabId);
                        //    var room = dm.GetOneRoom(jsonObject.RoomID);
                        //    if (player != null && room != null)
                        //    {
                        //        json = "{\"ResponseId\":\"YO\", \"MyData\":" + JsonConvert.SerializeObject(room) + "}";
                        //    }
                        //    else
                        //        Console.WriteLine("User ne postoji u dictionaryu. ne mogu se spojiti sobi");
                        //}
                    }
                    if (jsonObject.CommandId == "YOIMREADY")
                    {
                        var token = new TokenManager().DecryptToken(jsonObject.Jwt);
                        if (token != null)
                        {
                            DataManager dm = new DataManager();
                            dm.UpdatePlayerIngameStatus(jsonObject.PlayfabId, jsonObject.RoomID, "READY");
                            var room = dm.GetOneRoom(jsonObject.RoomID);
                            json = "{\"ResponseId\":\"OK\", \"MyData\":" + JsonConvert.SerializeObject(room) + "}";
                        }
                    }
                    if (jsonObject.CommandId == "YOSTARTGAME")
                    {
                        var token = new TokenManager().DecryptToken(jsonObject.Jwt);
                        if (token != null)
                        {
                            DataManager dm = new DataManager();
                            var players = dm.GetAllPlayers();
                            var rooms = dm.GetAllRooms();
                            var room = dm.GetOneRoom(jsonObject.RoomID);
                            bool readyToStart = true;
                            for (int i = 1; i < room.Players.Length; i++)
                            {
                                if (room.Players[i].InGameStatus != "READY")
                                    readyToStart = false;
                            }
                            var statusCode = "";
                            if (readyToStart)
                            {
                                for (int i = 0; i < room.Players.Length; i++)
                                {
                                    room.Players[i].InGameStatus = "GAME";
                                    var player = players[room.Players[i].PlayFabId];
                                    player.InGameStatus = "GAME";
                                    players[player.PlayFabId] = player;
                                }
                                dm.SetAllPlayers(players);
                                for (int i = 0; i < rooms.Count; i++)
                                {
                                    if (rooms[i].Id == room.Id)
                                        rooms[i] = room;
                                }
                                dm.SetAllRooms(rooms);
                                statusCode = "OK";
                            }
                            else
                                statusCode = "FORBIDDEN";
                            json = "{\"ResponseId\":\"" + statusCode + "\", \"MyData\":" + JsonConvert.SerializeObject(room) + "}";
                        }
                    }
                    if (jsonObject.CommandId == "YOIMKICKIN")
                    {
                        var token = new TokenManager().DecryptToken(jsonObject.Jwt);
                        if (token != null)
                        {
                            DataManager dm = new DataManager();
                            string statusCode = dm.RemovePlayerFromRoom(jsonObject.RoomID, jsonObject.PlayfabId, jsonObject.KickedPlayerId);
                            var room = dm.GetOneRoom(jsonObject.RoomID);
                            json = "{\"ResponseId\":\"" + statusCode + "\", \"MyData\":" + JsonConvert.SerializeObject(room) + "}";
                        }
                    }
                    if (jsonObject.CommandId == "YOGAMESTARTED")
                    {
                        var token = new TokenManager().DecryptToken(jsonObject.Jwt);
                        if (token != null)
                        {
                            DataManager dm = new DataManager();
                            var room = dm.GetOneRoom(jsonObject.RoomID);
                            dm.SetAllPlayersIngameStatus(room);
                            var game = new Game();
                            game.Players = room.Players;
                            var rand = new Random();
                            var dealer = rand.Next(0, 3);
                            game.DealerPlayfabId = game.Players[dealer].PlayFabId;
                            game.GameId = Guid.NewGuid();
                            game.Status = "STARTED";
                            game.RoomId = room.Id;
                            dm.AddOneGameToGames(game);
                            var statusCode = "OK";
                            json = "{\"ResponseId\":\"" + statusCode + "\", \"MyData\":" + JsonConvert.SerializeObject(game) + "}";
                        }
                    }
                    if (jsonObject.CommandId == "YOGIVEMEGAMEINFO")
                    {
                        var token = new TokenManager().DecryptToken(jsonObject.Jwt);
                        if (token != null)
                        {
                            DataManager dm = new DataManager();
                            Game game = dm.GetOneGame(jsonObject.GameId);
                            var statusCode = "OK";
                            json = "{\"ResponseId\":\"" + statusCode + "\", \"MyData\":" + JsonConvert.SerializeObject(game) + "}";
                        }
                    }
                    if (jsonObject.CommandId == "YOISHUFFLED")
                    {
                        var token = new TokenManager().DecryptToken(jsonObject.Jwt);
                        if (token != null)
                        {
                            DataManager dm = new DataManager();
                            var game = dm.GetOneGame(jsonObject.GameId);
                            game.ShuffledCards = jsonObject.Cards;
                            game.Status = "ROUNDSTART";
                            int index = -1;
                            for (int i = 0; i < game.Players.Length; i++)
                            {
                                if (game.Players[i].PlayFabId == game.DealerPlayfabId)
                                    index = i;
                            }
                            if (index != -1)
                            {
                                if (index == 0 || index == 1 || index == 2)
                                {
                                    index = index + 1;
                                    game.PlayerIdToPlay = game.Players[index].PlayFabId;
                                }
                                else
                                {
                                    index = 0;
                                    game.PlayerIdToPlay = game.Players[index].PlayFabId;
                                }
                            }
                            Card[] hand = new Card[10];
                            while (index < 4)
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    hand[i] = game.ShuffledCards[i];
                                }
                                game.Players[index].Hand = hand;
                                if (index < 4)
                                    index = index + 1;
                                else
                                    index = 0;
                                for (int i = 10; i < 20; i++)
                                {
                                    hand[i - 10] = game.ShuffledCards[i];
                                }
                                game.Players[index].Hand = hand;
                                if (index < 4)
                                    index = index + 1;
                                else
                                    index = 0;
                                for (int i = 20; i < 30; i++)
                                {
                                    hand[i - 20] = game.ShuffledCards[i];
                                }
                                game.Players[index].Hand = hand;
                                if (index < 4)
                                    index = index + 1;
                                else
                                    index = 0;
                                game.Players[index].Hand = hand;
                                for (int i = 30; i < 40; i++)
                                {
                                    hand[i - 30] = game.ShuffledCards[i];
                                }
                                game.Players[index].Hand = hand;
                            }
                            var games = dm.GetAllGames();
                            games[jsonObject.GameId] = game;
                            dm.SetAllGames(games);
                            var statusCode = "OK";
                            json = "{\"ResponseId\":\"" + statusCode + "\", \"MyData\":" + JsonConvert.SerializeObject(game) + "}";
                        }
                    }
                    if (jsonObject.CommandId == "YOIPLAYEDCARD")
                    {
                        var token = new TokenManager().DecryptToken(jsonObject.Jwt);
                        if (token != null)
                        {
                            DataManager dm = new DataManager();
                            var games = dm.GetAllGames();
                            var game = games[jsonObject.GameId];
                            var players = dm.GetAllPlayers();
                            var player = players[token.Id];
                            var newHand = player.Hand.Where(val => val.Value != jsonObject.CardValue).ToArray();
                            player.Hand = newHand;
                            players[token.Id] = player;
                            var index = -1;
                            for (int i = 0; i < game.Players.Length; i++)
                            {
                                if (game.Players[i].PlayFabId == game.DealerPlayfabId)
                                    index = i;
                            }
                            if (index != -1)
                            {
                                game.Players[index] = player;
                                if (index != 4)
                                    game.PlayerIdToPlay = game.Players[index + 1].PlayFabId;
                                else
                                    game.PlayerIdToPlay = game.Players[0].PlayFabId;
                            }
                            games[jsonObject.GameId] = game;
                            dm.SetAllPlayers(players);
                            dm.SetAllGames(games);
                            var statusCode = "OK";
                            json = "{\"ResponseId\":\"" + statusCode + "\", \"MyData\":" + JsonConvert.SerializeObject(game) + "}";
                        }
                    }
                    if (jsonObject.CommandId == "YOIMSENDINROOMMESSAGE")
                    {
                        return "";
                    }
                }
                return json;
            }catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
