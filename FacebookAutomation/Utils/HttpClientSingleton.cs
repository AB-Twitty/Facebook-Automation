using System.Net;

namespace FacebookAutomation.Utils
{
    public class HttpClientSingleton
    {
        private static readonly Lazy<HttpClientSingleton> _instance = new Lazy<HttpClientSingleton>(() => new HttpClientSingleton());

        public static HttpClientSingleton Instance => _instance.Value;

        public HttpClient HttpClient { get; }
        public bool IsConfigured { get; private set; } = false;

        private HttpClientSingleton()
        {
            HttpClient = new HttpClient();
        }

        public void ConfigureHttpClient(IReadOnlyCollection<OpenQA.Selenium.Cookie> cookies, string url)
        {
            HttpClient.DefaultRequestHeaders.Clear();

            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            HttpClient.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");

            var cookieContainer = new CookieContainer();

            foreach (var cookie in cookies)
            {
                var convertedCookie = ConvertToSystemNetCookie(cookie);

                cookieContainer.Add(new Uri(url), convertedCookie);
            }
            HttpClient.DefaultRequestHeaders.Add("Cookie", cookieContainer.GetCookieHeader(new Uri(url)));

            IsConfigured = true;
        }

        private Cookie ConvertToSystemNetCookie(OpenQA.Selenium.Cookie seleniumCookie)
        {
            return new Cookie(seleniumCookie.Name, seleniumCookie.Value)
            {
                Domain = seleniumCookie.Domain,
                Path = seleniumCookie.Path,
                Expires = seleniumCookie.Expiry ?? DateTime.MinValue,
                Secure = seleniumCookie.Secure
            };
        }
    }
}