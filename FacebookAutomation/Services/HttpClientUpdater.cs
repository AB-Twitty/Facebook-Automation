namespace FacebookAutomation.Services
{
    public abstract class HttpClientUpdater
    {
        protected HttpClient _httpClient;

        public virtual void UpdateHttpClient(HttpClient httpClient) => _httpClient = httpClient;
    }
}
