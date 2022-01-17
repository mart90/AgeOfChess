using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace AgeOfChess
{
    class MultiplayerApiClient
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public MultiplayerApiClient()
        {
            _baseUrl = "http://127.0.0.1:5000/";

            _client = new HttpClient()
            {
                BaseAddress = new Uri(_baseUrl)
            };
        }

        public int RegisterUser(string username, string plainTextPassword)
        {
            string password = HashPassword(plainTextPassword);
            return int.Parse(Get("register_user", new { username, password }));
        }

        public int? GetUserIdByName(string username)
        {
            var result = Get("get_user_id_by_name", new { username });
            return result != null ? int.Parse(result) : (int?)null;
        }

        public int? Login(string username, string password)
        {
            var result = Get("login", new { username, password });
            return result != null ? int.Parse(result) : (int?)null;
        }

        private string Get(string url, object convertableObject = null)
        {
            HttpRequestMessage request;
            if (convertableObject != null)
            {
                var jsonString = JsonConvert.SerializeObject(convertableObject);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                request = new HttpRequestMessage(HttpMethod.Get, url) { Content = content };
            }
            else
            {
                request = new HttpRequestMessage(HttpMethod.Get, url);
            }

            var response = _client.SendAsync(request).Result;
            var stringResult = response.Content.ReadAsStringAsync().Result;

            return stringResult == "None" ? null : stringResult;
        }

        public static string HashPassword(string password)
        {
            // TODO
            return password;
        }
    }
}
