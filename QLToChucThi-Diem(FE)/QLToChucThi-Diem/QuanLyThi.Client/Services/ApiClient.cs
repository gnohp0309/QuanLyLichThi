using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace QuanLyThi.Client.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private string? _token;

        public ApiClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/") // Update with your API URL
            };
        }

        public void SetToken(string token)
        {
            _token = token;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public void ClearToken()
        {
            _token = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/{endpoint}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content, JsonSettings);
            }
            catch
            {
                return default;
            }
        }

        public async Task<T?> PostAsync<T>(string endpoint, object? data = null)
        {
            try
            {
                var json = data != null ? JsonConvert.SerializeObject(data, JsonSettings) : null;
                var content = json != null ? new StringContent(json, Encoding.UTF8, "application/json") : null;
                
                var response = await _httpClient.PostAsync($"api/{endpoint}", content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseContent, JsonSettings);
            }
            catch
            {
                return default;
            }
        }

        public async Task<T?> PutAsync<T>(string endpoint, object? data = null)
        {
            try
            {
                var json = data != null ? JsonConvert.SerializeObject(data, JsonSettings) : null;
                var content = json != null ? new StringContent(json, Encoding.UTF8, "application/json") : null;
                
                var response = await _httpClient.PutAsync($"api/{endpoint}", content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseContent, JsonSettings);
            }
            catch
            {
                return default;
            }
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/{endpoint}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
