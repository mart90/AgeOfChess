using System.Net.Http;

namespace AgeOfChess
{
    class ApiClient
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public ApiClient()
        {
            _baseUrl = "https://127.0.0.1:5000/";
        }
    }
}
