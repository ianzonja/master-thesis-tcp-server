using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TCPserver
{
    public class BasicPlayerData
    {
        public string Wins { get; set; }
        public string Loses { get; set; }
        public string Draws { get; set; }
    }
    class UpdatePlayerData
    {
        public class UpdateUserDataRequest
        {
            public object Data { get; set; }
            public string PlayfabId { get; set; }
        }
        public class UpdateUserDataResponse
        {

        }
        public UpdatePlayerData(object playerData, string playfabId)
        {
            UpdateData(playerData, playfabId).Wait();
        }
        public UpdateUserDataResponse Response { get; set; }
        public async Task<UpdateUserDataResponse> UpdateData(object player, string playfabId)
        {
            var url = "https://6D8B1.playfabapi.com/Admin/UpdateUserData";
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                Console.WriteLine("");
                UpdateUserDataRequest body = new UpdateUserDataRequest();
                body.Data = player;
                body.PlayfabId = playfabId;
                if (playfabId != null)
                {
                    var json = JsonConvert.SerializeObject(body);
                    using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        request.Content = stringContent;
                        request.Headers.Add("X-SecretKey", "CEZ45YQZEBQFFJF1ENKAYDBQUFPFCQOT5QC6I9ZI7WM7IHGSQ6");
                        using (var response = await client
                            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                            .ConfigureAwait(false))
                        {
                            response.EnsureSuccessStatusCode();
                            HttpContent requestContent = response.Content;
                            var stringResponse = await requestContent.ReadAsStringAsync();
                            UpdateUserDataResponse res = JsonConvert.DeserializeObject<UpdateUserDataResponse>(stringResponse);
                            this.Response = res;
                            return res;
                        }
                    }
                }
                else
                {
                    return new UpdateUserDataResponse();
                }
                return new UpdateUserDataResponse();
            }
        }
    }
}