using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.AuthenticationModels;
using PlayFab.ClientModels;
using PlayFab.Internal;
using PlayFab.ServerModels;

namespace TCPserver
{
    public class TitlePlayerAccount
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string TypeString { get; set; }
    }

    public class TitleInfo
    {
        public string Origination { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime FirstLogin { get; set; }
        public bool isBanned { get; set; }
        public TitlePlayerAccount TitlePlayerAccount { get; set; }
    }

    public class PrivateInfo
    {
        public string Email { get; set; }
    }

    public class UserInfo
    {
        public string PlayFabId { get; set; }
        public DateTime Created { get; set; }
        public string Username { get; set; }
        public TitleInfo TitleInfo { get; set; }
        public PrivateInfo PrivateInfo { get; set; }
    }

    public class Data
    {
        public UserInfo UserInfo { get; set; }
        public bool IsSessionTicketExpired { get; set; }
    }

    public class AuthResponse
    {
        public int code { get; set; }
        public string status { get; set; }
        public Data data { get; set; }
    }
    public class AuthenticateSessionTicketPostBody{
        public string sessionTicket { get; set; }
    }
    public class ValidateClient
    {
        static readonly object _object = new object();
        public AuthResponse Response { get; set; }
        public ValidateClient(string sessionTicket)
        {
            this.ValidateSessionTicket(sessionTicket).Wait();
        }

        private async Task<AuthResponse> ValidateSessionTicket(string sessionTicket)
        {
            try
            {
                var url = "https://6D8B1.playfabapi.com/Server/AuthenticateSessionTicket";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    Console.WriteLine(sessionTicket);
                    AuthenticateSessionTicketPostBody body = new AuthenticateSessionTicketPostBody();
                    body.sessionTicket = sessionTicket;
                    var json = JsonConvert.SerializeObject(body);
                    using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        request.Content = stringContent;
                        request.Headers.Add("X-SecretKey", "CEZ45YQZEBQFFJF1ENKAYDBQUFPFCQOT5QC6I9ZI7WM7IHGSQ6");
                        using (var response = await client
                            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                            .ConfigureAwait(false))
                        {
                            Monitor.Enter(_object);
                            response.EnsureSuccessStatusCode();
                            HttpContent requestContent = response.Content;
                            var stringResponse = await requestContent.ReadAsStringAsync();
                            AuthResponse res = JsonConvert.DeserializeObject<AuthResponse>(stringResponse);
                            this.Response = res;
                            Monitor.Exit(_object);
                            return res;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new AuthResponse();
            }
        }

    }
}
