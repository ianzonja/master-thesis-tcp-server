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
        private string[] poruke = {};
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
                if(jsonObject.CommandId == "YOIJUSTLOGGEDIN")
                {
                    Console.WriteLine("eurekica");
                    ValidateClient client = new ValidateClient(jsonObject.SessionTicket);
                    if(client.Response != null)
                    {
                        if(client.Response.code == 200)
                        {
                            Console.WriteLine("koji k");
                            Console.WriteLine(client.Response.data.UserInfo.Username);
                            Player player = new Player();
                            player.Email = client.Response.data.UserInfo.PrivateInfo.Email;
                            player.Username = client.Response.data.UserInfo.Username;
                            player.PlayFabId = client.Response.data.UserInfo.PlayFabId;
                            GetUserData data = new GetUserData(player.PlayFabId);
                            if(data.Response != null)
                            {
                                player.Loses = data.Response.data.Data.Loses.Value;
                                player.Wins = data.Response.data.Data.Wins.Value;
                                player.Draws = data.Response.data.Data.Draws.Value;
                            }
                            Singleton.Instance.AddPlayerToConnectedPlayersDictionary(player);
                            json = "{\"ResponseId\":\"YO\", \"MyData\":"+JsonConvert.SerializeObject(player)+"}";
                            Console.WriteLine(json);
                        }
                    }
                }
                if (jsonObject.CommandId == "YOGIVEMEROOMINFO")
                {
                    ValidateClient client = new ValidateClient(jsonObject.SessionTicket);
                    if (client.Response != null)
                    {
                        if (client.Response.code == 200)
                        {
                            if (Singleton.Instance.GetPlayers().ContainsKey(client.Response.data.UserInfo.PlayFabId))
                            {
                                string array = "[";
                                for (int i = 0; i < Singleton.Instance.GetPlayers().Values.Count; i++)
                                {
                                    foreach(var item in Singleton.Instance.GetPlayers())
                                    {
                                        Console.WriteLine("item");
                                    }
                                    array += JsonConvert.SerializeObject(Singleton.Instance.GetPlayers()) + ",";
                                }
                                array = array.Remove(array.Length - 1);
                                array += "]";
                                DataManager dm = new DataManager();
                                var players = dm.GetAllPlayers();
                                var currentPlayer = players[client.Response.data.UserInfo.PlayFabId];
                                string playersJson = JsonConvert.SerializeObject(players.Values, Formatting.Indented);
                                var rooms = dm.GetAllRooms();
                                string roomsJson = JsonConvert.SerializeObject(rooms);
                                json = "{\"NumberOfPlayers\":\""+players.Values.Count+"\", \"Players\":"+playersJson + ", \"Rooms\": "+ roomsJson + "}";
                                Console.WriteLine(json);
                            }
                        }
                    }
                }
                if(jsonObject.CommandId == "YOIJUSTREGISTERED")
                {
                    ValidateClient client = new ValidateClient(jsonObject.SessionTicket);
                    if(client.Response != null)
                    {
                        BasicPlayerData pbd = new BasicPlayerData();
                        pbd.Draws = "0";
                        pbd.Loses = "0";
                        pbd.Wins = "0";
                        UpdatePlayerData updatePlayerData = new UpdatePlayerData(pbd, client.Response.data.UserInfo.PlayFabId);
                        Player player = new Player();
                        player.Email = client.Response.data.UserInfo.PrivateInfo.Email;
                        player.Username = client.Response.data.UserInfo.Username;
                        player.PlayFabId = client.Response.data.UserInfo.PlayFabId;
                        GetUserData data = new GetUserData(player.PlayFabId);
                        if (data.Response != null)
                        {
                            player.Loses = data.Response.data.Data.Loses.Value;
                            player.Wins = data.Response.data.Data.Wins.Value;
                            player.Draws = data.Response.data.Data.Draws.Value;
                        }
                        Singleton.Instance.AddPlayerToConnectedPlayersDictionary(player);
                        json = "{\"ResponseId\":\"YO\", \"MyData\":" + JsonConvert.SerializeObject(player) + "}";
                    }
                }
                if(jsonObject.CommandId == "YOIWANNAHOST")
                {
                    ValidateClient client = new ValidateClient(jsonObject.SessionTicket);
                    if (client.Response != null)
                    {
                        Room room = new Room();
                        room.HostPlayfabId = client.Response.data.UserInfo.PlayFabId;
                        room.Id = Guid.NewGuid().ToString();
                        var players = new DataManager().GetAllPlayers();
                        Player[] roomPlayers = new Player[1];
                        roomPlayers[0] = players[client.Response.data.UserInfo.PlayFabId];
                        room.Players = roomPlayers;
                        json = "{\"ResponseId\":\"YO\", \"MyData\":" + JsonConvert.SerializeObject(room) + "}";
                        Singleton.Instance.AddRoom(room);
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Na prepoznajem poruku");
            }
            return json;
        }
    }
}
