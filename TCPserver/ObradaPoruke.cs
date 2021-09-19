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
                }
                if (jsonObject.CommandId == "YOIWANNAHOST")
                {
                    var token = new TokenManager().DecryptToken(jsonObject.Jwt);
                    if(token != null)
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
                    if(token != null)
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
                    if(token != null)
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
                    ValidateClient client = new ValidateClient(jsonObject.SessionTicket);
                    if (client.Response != null)
                    {
                        DataManager dm = new DataManager();
                        dm.UpdatePlayerIngameStatus(jsonObject.PlayfabId, jsonObject.RoomID, "READY");
                        var room = dm.GetOneRoom(jsonObject.RoomID);
                        json = "{\"ResponseId\":\"OK\", \"MyData\":" + JsonConvert.SerializeObject(room) + "}";
                    }
                }
                if (jsonObject.CommandId == "YOSTARTGAME")
                {
                    ValidateClient client = new ValidateClient(jsonObject.SessionTicket);
                    if (client.Response != null)
                    {
                        DataManager dm = new DataManager();
                        var statusCode = dm.SetPlayersStatusIngame(jsonObject.PlayfabId, jsonObject.RoomID);
                        var room = dm.GetOneRoom(jsonObject.RoomID);
                        json = "{\"ResponseId\":\"" + statusCode + "\", \"MyData\":" + JsonConvert.SerializeObject(room) + "}";
                    }
                }
                if (jsonObject.CommandId == "YOIMKICKIN")
                {
                    ValidateClient client = new ValidateClient(jsonObject.SessionTicket);
                    if (client.Response != null)
                    {
                        DataManager dm = new DataManager();
                        string statusCode = dm.RemovePlayerFromRoom(jsonObject.RoomID, jsonObject.PlayfabId, jsonObject.KickedPlayerId);
                        var room = dm.GetOneRoom(jsonObject.RoomID);
                        json = "{\"ResponseId\":\"" + statusCode + "\", \"MyData\":" + JsonConvert.SerializeObject(room) + "}";
                    }
                }
                if (jsonObject.CommandId == "YOIMSENDINROOMMESSAGE")
                {
                    return "";
                }
                return json;
            }catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
