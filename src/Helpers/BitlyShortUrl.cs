using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CsCopilot.Helpers
{
    public class BitlyShortUrl
    {
        public BitlyShortUrl(string token)
        {
            this.token = token;
            httpCLient = new HttpClient { BaseAddress = new Uri("https://api-ssl.bitly.com/") };
        }
        private readonly string token;
        private readonly HttpClient httpCLient;

        public async Task<string> ShortenAsync(string longUrl)
        {
            try
            {
                var postContent = new { long_url = longUrl };

                httpCLient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var response = await httpCLient.PostAsync("v4/shorten", 
                    new StringContent(JsonSerializer.Serialize(postContent), Encoding.UTF8,
                        "application/json"));
               
                var stringResponse = await response.Content.ReadAsStringAsync();
                var bitlyResp = JsonSerializer.Deserialize<bitlyResp>(stringResponse);
                return bitlyResp.link;
            }
            catch (WebException ex)
            {
                return ex.Message;
            }
        }
    }

    public class References
    {
        public string group { get; set; }
    }

    public class bitlyResp
    {
        public string id { get; set; }
        public string link { get; set; }
        public List<object> custom_bitlinks { get; set; }
        public string long_url { get; set; }
        public bool archived { get; set; }
        public List<object> tags { get; set; }
        public List<object> deeplinks { get; set; }
        public References references { get; set; }
    }


}
