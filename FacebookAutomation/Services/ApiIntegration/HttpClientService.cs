using System.Net;
using System.Text;
using System.Text.Json;

namespace FacebookAutomation.Services.ApiIntegration
{
    public class HttpClientService
    {
        private static readonly Lazy<HttpClientService> _instance = new(() => new HttpClientService());
        private readonly HttpClient _httpClient;

        private HttpClientService()
        {
            _httpClient = new HttpClient();
        }

        public static HttpClientService Instance => _instance.Value;

        private static string AddQueryParameters(string url, Dictionary<string, string> queryParameters)
        {
            if (queryParameters == null || queryParameters.Count == 0)
                return url;

            var query = new FormUrlEncodedContent(queryParameters).ReadAsStringAsync().Result;
            return $"{url}?{query}";
        }

        public async Task<T> GetAsync<T>(string url, Dictionary<string, string> queryParameters = null)
        {
            url = AddQueryParameters(url, queryParameters);
            var response = await _httpClient.GetAsync(url);
            return await HandleResponse<T>(response);
        }

        public async Task<T> PostAsync<T>(string url, object data = null, Dictionary<string, string> queryParameters = null, string contentType = "application/json")
        {
            url = AddQueryParameters(url, queryParameters);
            var content = data != null
                ? new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, contentType)
                : null;

            var response = await _httpClient.PostAsync(url, content);
            return await HandleResponse<T>(response);
        }

        public async Task<T> PutAsync<T>(string url, object data = null, Dictionary<string, string> queryParameters = null, string contentType = "application/json")
        {
            url = AddQueryParameters(url, queryParameters);
            var content = data != null
        ? new FormUrlEncodedContent(data as Dictionary<string, string>)  // Convert object data to form encoded
        : null;

            var response = await _httpClient.PutAsync(url, content);
            return await HandleResponse<T>(response);
        }

        public async Task<T> DeleteAsync<T>(string url, Dictionary<string, string> queryParameters = null)
        {
            url = AddQueryParameters(url, queryParameters);
            var response = await _httpClient.DeleteAsync(url);
            return await HandleResponse<T>(response);
        }

        private async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponse(response);
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        private async Task HandleErrorResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new HttpRequestException($"Bad Request: {content}");
                case HttpStatusCode.Unauthorized:
                    throw new HttpRequestException("Unauthorized access to the external service.");
                case HttpStatusCode.Forbidden:
                    throw new HttpRequestException("Access to the external service is forbidden.");
                case HttpStatusCode.NotFound:
                    throw new HttpRequestException("The requested resource was not found on the external service.");
                case HttpStatusCode.TooManyRequests:
                    throw new HttpRequestException("Too many requests to the external service. Please try again later.");
                case HttpStatusCode.InternalServerError:
                    throw new HttpRequestException("Error processing request from the external service.");
                default:
                    throw new HttpRequestException($"Unexpected status code ({(int)response.StatusCode}): {content}");
            }
        }
    }
}
