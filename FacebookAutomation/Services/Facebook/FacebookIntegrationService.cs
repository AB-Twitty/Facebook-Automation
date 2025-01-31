using FacebookAutomation.Contracts.IFacebook;
using FacebookAutomation.Models.Facebook;
using FacebookAutomation.Utils;

namespace FacebookAutomation.Services.Facebook
{
    public abstract class FacebookIntegrationService<TModel> : HttpClientUpdater, IFacebookIntegrationService where TModel : BaseResponseModel
    {
        protected readonly Dictionary<string, string> _basicFormData;
        protected const string Url = "https://www.facebook.com/api/graphql/";

        public FacebookIntegrationService()
        {
            _httpClient = HttpClientSingleton.Instance.HttpClient;

            if (!HttpClientSingleton.Instance.IsConfigured)
            {
                TryReLoginAsync().Wait();
            }

            _basicFormData = FormDataState.Instance.GetAllFormData();
        }

        protected async Task TryReLoginAsync()
        {
            try
            {
                Console.WriteLine("Attempting to re-login...");
                await FacebookLogoutAutomation.Logout();
                var cookies = FacebookLoginAutomation.Login();
                HttpClientSingleton.Instance.ConfigureHttpClient(cookies, Url);

                if (HttpClientSingleton.Instance.IsProxyChanged)
                {
                    _httpClient.Dispose();
                    UpdateHttpClient(HttpClientSingleton.Instance.HttpClient);
                    HttpClientSingleton.Instance.IsProxyChanged = false;
                }

                //await SetDtsgTokenAsync();

                Console.WriteLine("Re-login successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Re-login failed: {ex.Message}. Retrying...");

                await Task.Delay(30 * 1000); // delay for 30 seconds
                await TryReLoginAsync();
            }
        }

        // Abstract methods that need to be implemented by subclasses
        public abstract Task<BaseResponse<BaseResponseModel>> SendSearchRequestAsync(string search, Pagination? nextPage = null);
        public abstract Task<BaseResponse<FacebookUser>> GetFacebookUsersFor(BaseResponseModel model, Pagination? nextPage = null, int limit = 0);




        /*
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
                { "doc_id", docId.ToString() }
            };

            var content = new FormUrlEncodedContent(extraFormData.Concat(_basicFormData));
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
                        //FormDataState.Instance.Fb_Dtsg = jsonObject["dtsgToken"]?.ToString()
                        //  ?? FormDataState.Instance.Fb_Dtsg;
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
         
         */
    }
}