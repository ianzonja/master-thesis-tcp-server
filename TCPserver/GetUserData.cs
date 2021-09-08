using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPserver
{
    public class GetUserDataRequest
    {
        public object Data { get; set; }
        public string PlayfabId { get; set; }
    }
    public class Draws
    {
        public string Value { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Permission { get; set; }
    }

    public class Email
    {
        public string Value { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Permission { get; set; }
    }

    public class Loses
    {
        public string Value { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Permission { get; set; }
    }

    public class PlayFabId
    {
        public string Value { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Permission { get; set; }
    }

    public class Username
    {
        public string Value { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Permission { get; set; }
    }

    public class Wins
    {
        public string Value { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Permission { get; set; }
    }

    public class PlayerData
    {
        public Draws Draws { get; set; }
        public Email Email { get; set; }
        public Loses Loses { get; set; }
        public PlayFabId PlayFabId { get; set; }
        public Username Username { get; set; }
        public Wins Wins { get; set; }
    }

    public class data
    {
        public string PlayfabId { get; set; }

        public string DataVersion { get; set; }
        public PlayerData Data { get; set; }
    }

    public class GetUserDataResponse
    {
        public int code { get; set; }
        public string status { get; set; }
        public data data { get; set; }
    }
    public class GetUserData
    {
        static readonly object _object = new object();

        public GetUserData(string playfabId)
        {
            Console.WriteLine("I am here before");
            GetData(playfabId).Wait();
            Console.WriteLine("I am on the next line");
        }
        public GetUserDataResponse Response { get; set; }
        public async Task<GetUserDataResponse> GetData(string playfabId)
        {
            try
            {
                Console.WriteLine("I am on begining of method");
                var url = "https://6D8B1.playfabapi.com/Server/GetUserData";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    Console.WriteLine("2");
                    Console.WriteLine("size of dictionary");
                    DataManager dm = new DataManager();
                    Console.WriteLine(dm.GetAllPlayers().Count);
                    GetUserDataRequest body = new GetUserDataRequest();
                    body.PlayfabId = playfabId;
                    Console.WriteLine("loop");
                    if (playfabId != null)
                    {
                        Console.WriteLine("4");
                        var json = JsonConvert.SerializeObject(body);
                        using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                        {
                            Console.WriteLine("5");
                            request.Content = stringContent;
                            request.Headers.Add("X-SecretKey", "CEZ45YQZEBQFFJF1ENKAYDBQUFPFCQOT5QC6I9ZI7WM7IHGSQ6");
                            using (var response = await client
                                .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                                .ConfigureAwait(false))
                            {
                                Console.WriteLine("I am inside call");
                                response.EnsureSuccessStatusCode();
                                HttpContent requestContent = response.Content;
                                var stringResponse = await requestContent.ReadAsStringAsync();
                                this.Response = JsonConvert.DeserializeObject<GetUserDataResponse>(stringResponse);
                                return this.Response;
                            }
                        }
                    }
                    else
                    {
                        return new GetUserDataResponse();
                    }
                    return new GetUserDataResponse();
                }
            }
            catch(Exception ex)
            {
                Console.Write(ex.ToString());
                return new GetUserDataResponse();
            }
        }
    }
}
