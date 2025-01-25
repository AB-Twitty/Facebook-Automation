using FacebookAutomation.Contracts.IFacebook;
using FacebookAutomation.Models.Facebook;
using FacebookAutomation.Utils;
using Newtonsoft.Json.Linq;
using System.Net;

namespace FacebookAutomation.Services.Facebook
{
    public abstract class FacebookIntegrationService<TModel> : IFacebookIntegrationService where TModel : BaseResponseModel
    {
        protected readonly HttpClient _httpClient;
        protected const string Url = "https://www.facebook.com/api/graphql/";
        private const string Lsd = "AVo2Q6Qv";
        private const string Jazoest = "22058";
        private const bool ServerTimestamps = false;
        private string Fb_Dtsg = "NAcMuQSLToj3CwhPmzPObc4PbnO23ShE7gLiHa6XurgY6ONI_xrXKgw:32:1737666094"; // Default value for dtsgToken

        public FacebookIntegrationService()
        {
            _httpClient = new HttpClient();

            var cookies = FacebookLoginAutomation.Login();
            ConfigureHttpClient(cookies).Wait();
        }

        private async Task ConfigureHttpClient(IReadOnlyCollection<OpenQA.Selenium.Cookie> cookies)
        {
            _httpClient.DefaultRequestHeaders.Clear();

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");

            var cookieContainer = new CookieContainer();

            foreach (var cookie in cookies)
            {
                cookieContainer.Add(new Uri(Url), ConvertToSystemNetCookie(cookie));
            }

            _httpClient.DefaultRequestHeaders.Add("Cookie", cookieContainer.GetCookieHeader(new Uri(Url)));

            await SetDtsgTokenAsync();
        }

        protected async Task SetDtsgTokenAsync()
        {
            try
            {
                await SendDummyRequestToSetDtsgTokenAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during dtsgToken setup: {ex.Message}", ex);
            }
        }

        private async Task<HttpResponseMessage> SendDummyRequestAsync()
        {
            const string DUMMY_POST_FEEDBACK_ID = "ZmVlZGJhY2s6MTIwMjQ0NjYwNzkxOTY1Nw==";

            var fbApiReqFriendlyName = "CometUFIReactionsDialogTabContentRefetchQuery";
            var variables = "{\"count\":15,\"cursor\":null,\"feedbackTargetID\":\"" + DUMMY_POST_FEEDBACK_ID + "\",\"id\":\"" + DUMMY_POST_FEEDBACK_ID + "\"}";
            var docId = 9069004929826853;

            var extraFormData = new Dictionary<string, string>
            {
                { "fb_api_req_friendly_name", fbApiReqFriendlyName },
                { "variables", variables },
                { "doc_id", docId.ToString() },
                { "__a", "1" }
            };

            var content = new FormUrlEncodedContent(extraFormData.Concat(GetBaseFormData()));
            var response = await _httpClient.PostAsync(Url, content);

            return response;
        }

        private async Task SendDummyRequestToSetDtsgTokenAsync()
        {
            var response = await SendDummyRequestAsync();

            if (response.IsSuccessStatusCode && response.Content.Headers.ContentType.MediaType == "application/x-javascript")
            {
                try
                {
                    string jsonPart = await Helper.ExtractJsonFromJavascript(response);

                    if (!string.IsNullOrEmpty(jsonPart))
                    {
                        var jsonObject = JObject.Parse(jsonPart);
                        Fb_Dtsg = jsonObject["dtsgToken"]?.ToString() ?? Fb_Dtsg;
                    }
                    else
                    {
                        Console.WriteLine("Failed to extract JSON from response.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing the response: {ex.Message}", ex);
                }
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            }
        }

        protected Dictionary<string, string> GetBaseFormData()
        {
            return new Dictionary<string, string>
            {
                { "fb_dtsg", Fb_Dtsg },
                { "server_timestamps", ServerTimestamps.ToString() },
                { "lsd", Lsd },
                { "jazoest", Jazoest }
            };
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

        protected async Task TryReLoginAsync()
        {
            try
            {
                Console.WriteLine("Attempting to re-login...");

                var cookies = FacebookLoginAutomation.Login();
                await ConfigureHttpClient(cookies);

                Console.WriteLine("Re-login successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Re-login failed: {ex.Message}. Retrying...");

                await Task.Delay(5000);
                await TryReLoginAsync();
            }
        }

        // Abstract methods that need to be implemented by subclasses
        public abstract Task<BaseResponse<BaseResponseModel>> SendSearchRequestAsync(string search, PageInfo? nextPage = null);
        public abstract Task<BaseResponse<FacebookUser>> GetFacebookUsersFor(BaseResponseModel model, PageInfo? nextPage = null);
    }
}
