using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RichText.Handlers
{
    public class BaseHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BaseHandler(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected async Task<string> GetRequestAsync(string path)
        {
            using var httpClient = _httpClientFactory.CreateClient();

            using var request = CreateMethod(HttpMethod.Get, path);

            using var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        protected Task<string> PostRequestAsync<T>(string path, T content) => SendRequestAsync(HttpMethod.Post, path, content);

        protected Task<string> PutRequestAsync<T>(string path, T content) => SendRequestAsync(HttpMethod.Put, path, content);

        protected Task DeleteRequestAsync(string path) => SendRequestAsync(HttpMethod.Delete, path);

        private async Task<string> SendRequestAsync<T>(HttpMethod method, string path, T content)
        {
            using var httpClient = _httpClientFactory.CreateClient();

            using var request = CreateMethod(method, path);
            request.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            using var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private async Task SendRequestAsync(HttpMethod method, string path)
        {
            using var httpClient = _httpClientFactory.CreateClient();

            using var request = CreateMethod(method, path);
            using var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        private static HttpRequestMessage CreateMethod(HttpMethod method, string path)
        {
        }

    }
}
