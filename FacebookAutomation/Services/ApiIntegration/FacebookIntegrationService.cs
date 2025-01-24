using FacebookAutomation.Models;
using Newtonsoft.Json.Linq;
using System.Net;

namespace FacebookAutomation.Services.ApiIntegration
{
    public class FacebookIntegrationService
    {
        private readonly HttpClient _httpClient;
        private const string url = "https://www.facebook.com/api/graphql/";
        private const string lsd = "AVo2Q6Qv";
        private const string jazoest = "22058";
        private const bool server_timestamps = false;
        public FacebookIntegrationService()
        {
            // Set headers, including the cookie and User-Agent for simulating the browser
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");

            // Add cookies to HttpClient using CookieContainer if necessary
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Uri(url), new Cookie("wd", "678x746"));
            cookieContainer.Add(new Uri(url), new Cookie("dpr", "1.25"));
            cookieContainer.Add(new Uri(url), new Cookie("datr", "kKSSZ3QHGInbvEP_XXHMo3Wk"));
            cookieContainer.Add(new Uri(url), new Cookie("sb", "5KSSZw00mVeHT4utTOjvkz5O"));
            cookieContainer.Add(new Uri(url), new Cookie("ps_l", "1"));
            cookieContainer.Add(new Uri(url), new Cookie("ps_n", "1"));
            cookieContainer.Add(new Uri(url), new Cookie("c_user", "100006768661287"));
            cookieContainer.Add(new Uri(url), new Cookie("xs", "32%3A3_q9xwwMWqSecA%3A2%3A1737666094%3A-1%3A8592"));
            cookieContainer.Add(new Uri(url), new Cookie("presence", "C%7B%22t3%22%3A%5B%5D%2C%22utc3%22%3A1737666103168%2C%22v%22%3A1%7D"));
            cookieContainer.Add(new Uri(url), new Cookie("fr", "03qjofaADqZG5cbO6.AWWVjuzTeV5ANYBDvITlomRTgdE.BnkqTk..AAA.0.0.Bnkq-p.AWXrV54aJ5A"));
            _httpClient.DefaultRequestHeaders.Add("Cookie", cookieContainer.GetCookieHeader(new Uri(url)));

        }

        public async Task Login()
        {
            var email = "bobofawzy4@gmail.com";
            var encpass = "#PWD_BROWSER:5:1737662720:AXRQALUAAczJ2qbtfIlgTo7CGW6rG6E/sp507ePH3bquZ4VJtIa+o5mvLE45NK+2u3MsJYInqIu39fkI4lo1TFW84C+r9clICEiXcJNVB0udhKafZQSY0ohQG7TyTxKVdI8UCxlSO9++H48VvWHjE7I=";
            var login_source = "comet_headerless_login";

            // Create a HttpClientHandler to manage cookies
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer
            };

            var httpClient = new HttpClient(handler); // Pass the handler to the HttpClient

            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            var loginUrl = "https://www.facebook.com/login";

            // Form data for the login request
            var loginData = new Dictionary<string, string>
            {
                { "email", email },
                { "encpass", encpass },
                { "login_source", login_source },
                { "lsd", lsd },
                { "jazoest", jazoest }
            };

            // Set headers for the POST request, especially for content type
            var content = new FormUrlEncodedContent(loginData);

            // Perform the POST request to login
            var loginResponse = await httpClient.PostAsync(loginUrl, content);

            // Handle the response
            if (loginResponse.IsSuccessStatusCode)
            {
                // Optionally, retrieve the cookies after a successful login
                Uri uri = new Uri(loginUrl);
                var cookies = cookieContainer.GetCookies(uri);

                // Iterate over the cookies and print them out or store them for further use
                foreach (Cookie cookie in cookies)
                {
                    Console.WriteLine($"Cookie: {cookie.Name} = {cookie.Value}");
                }

                // Process the response (for example, read the page content or check login state)
                var responseContent = await loginResponse.Content.ReadAsStringAsync();
                // You can process this further if needed
            }
            else
            {
                // Handle failed login attempt
                Console.WriteLine("Login failed");
            }
        }

        public async Task GetReatorsForPostByFeedbackId(string feedbackId)
        {
            // Form data as URL-encoded
            var fb_dtsg = "NAcMuQSLToj3CwhPmzPObc4PbnO23ShE7gLiHa6XurgY6ONI_xrXKgw:32:1737666094";
            var fb_api_req_friendly_name = "CometUFIReactionsDialogTabContentRefetchQuery";
            var variables = "{\"count\":15,\"cursor\":null,\"feedbackTargetID\":" + $"\"{feedbackId}\"" + ",\"id\":" + $"\"{feedbackId}\"" + "}";
            var doc_id = 9069004929826853;

            // Create a dictionary for the form data (URL-encoded)
            var formData = new Dictionary<string, string>
            {
                { "fb_dtsg", fb_dtsg },
                { "fb_api_req_friendly_name", fb_api_req_friendly_name },
                { "variables", variables },
                { "doc_id", doc_id.ToString() },
                { "server_timestamps", server_timestamps.ToString() },
                { "lsd", lsd },
                { "jazoest", jazoest }
            };

            // Prepare the content for the POST request
            var content = new FormUrlEncodedContent(formData);

            // Send the POST request
            var response = await _httpClient.PostAsync(url, content);

            // Check if the response was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response as string
                var responseContent = await response.Content.ReadAsStringAsync();

                // Deserialize the response (you can define the structure of the response based on your API)
                // Assuming the response is a JSON object, you can create a class to hold the data
                string[] lines = responseContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                // Get the first line (which is the first JSON object)
                string firstJsonObject = lines[0].Trim();

                // Deserialize the response (you can define the structure of the response based on your API)
                // Assuming the response is a JSON object, you can create a class to hold the data
                JObject jsonObj = JObject.Parse(firstJsonObject);

                var post_reactions_mapper = new PostReactionsMapper();

                var postReactionsResult = post_reactions_mapper.MapToPostInfoResult(jsonObj);


                foreach (var reator in postReactionsResult.Reactors)
                {
                    Console.WriteLine($"Reactor: {reator.Name}       Id: {reator.Id}");
                }
            }
            else
            {
                Console.WriteLine("Request failed with status code: " + response.StatusCode);
            }
        }


        public async Task SearchQueryForPosts(string search, string? location = null, bool select_recent = true)
        {
            // Form data as URL-encoded
            var fb_dtsg = "NAcMuQSLToj3CwhPmzPObc4PbnO23ShE7gLiHa6XurgY6ONI_xrXKgw:32:1737666094";
            var fb_api_req_friendly_name = "SearchCometResultsPaginatedResultsQuery";


            var variables = "{\"allow_streaming\":false,\"args\":{\"callsite\":\"COMET_GLOBAL_SEARCH\",\"config\":{\"exact_match\":true,\"high_confidence_config\":null,\"intercept_config\":null,\"sts_disambiguation\":null,\"watch_config\":null},\"context\":null,\"experience\":{\"client_defined_experiences\":[\"ADS_PARALLEL_FETCH\"],\"encoded_server_defined_params\":null,\"fbid\":null,\"type\":\"POSTS_TAB\"},\"filters\":[],\"text\":\"" + search + "\"},\"count\":5,\"cursor\":null,\"feedLocation\":\"SEARCH\",\"feedbackSource\":23,\"fetch_filters\":true,\"focusCommentID\":null,\"locale\":null,\"privacySelectorRenderLocation\":\"COMET_STREAM\",\"renderLocation\":\"search_results_page\",\"scale\":1,\"stream_initial_count\":0,\"useDefaultActor\":false,\"__relay_internal__pv__GHLShouldChangeAdIdFieldNamerelayprovider\":false,\"__relay_internal__pv__GHLShouldChangeSponsoredDataFieldNamerelayprovider\":false,\"__relay_internal__pv__IsWorkUserrelayprovider\":false,\"__relay_internal__pv__CometFeedStoryDynamicResolutionPhotoAttachmentRenderer_experimentWidthrelayprovider\":500,\"__relay_internal__pv__CometImmersivePhotoCanUserDisable3DMotionrelayprovider\":false,\"__relay_internal__pv__WorkCometIsEmployeeGKProviderrelayprovider\":false,\"__relay_internal__pv__IsMergQAPollsrelayprovider\":false,\"__relay_internal__pv__FBReelsMediaFooter_comet_enable_reels_ads_gkrelayprovider\":false,\"__relay_internal__pv__CometUFIReactionsEnableShortNamerelayprovider\":false,\"__relay_internal__pv__CometUFIShareActionMigrationrelayprovider\":true,\"__relay_internal__pv__StoriesArmadilloReplyEnabledrelayprovider\":true,\"__relay_internal__pv__EventCometCardImage_prefetchEventImagerelayprovider\":false}";

            var doc_id = 28118757241106260;

            // Create a dictionary for the form data (URL-encoded)
            var formData = new Dictionary<string, string>
            {
                { "fb_dtsg", fb_dtsg },
                { "fb_api_req_friendly_name", fb_api_req_friendly_name },
                { "variables", variables },
                { "doc_id", doc_id.ToString() },
                { "server_timestamps", server_timestamps.ToString() },
                { "lsd", lsd },
                { "jazoest", jazoest }
            };


            // Prepare the content for the POST request
            var content = new FormUrlEncodedContent(formData);

            // Send the POST request
            var response = await _httpClient.PostAsync(url, content);

            // Check if the response was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response as string
                var responseContent = await response.Content.ReadAsStringAsync();

                string[] lines = responseContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                // Get the first line (which is the first JSON object)
                string firstJsonObject = lines[0].Trim();

                // Deserialize the response (you can define the structure of the response based on your API)
                // Assuming the response is a JSON object, you can create a class to hold the data
                JObject jsonObj = JObject.Parse(firstJsonObject);

                var post_mapper = new PostMapper();

                var postInfoResults = post_mapper.MapToPostInfoResult(jsonObj);

                // Process the response data (example)
                Console.WriteLine("Response: " + responseContent);
            }
            else
            {
                Console.WriteLine("Request failed with status code: " + response.StatusCode);
            }
        }

    }
}
