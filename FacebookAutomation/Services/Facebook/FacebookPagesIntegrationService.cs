using FacebookAutomation.Exceptions;
using FacebookAutomation.Mapping;
using FacebookAutomation.Models.Facebook;
using FacebookAutomation.Utils;
using OpenQA.Selenium.Chrome;
using System.Text.RegularExpressions;

namespace FacebookAutomation.Services.Facebook
{
    public class FacebookPagesIntegrationService : FacebookIntegrationService<PageInfoModel>
    {
        public FacebookPagesIntegrationService() : base()
        {
        }

        public static PageInfoModel? GetPageInfoFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless"); // Run in headless mode
            options.AddArgument("--disable-gpu"); // Disable GPU
            options.AddArgument("--no-sandbox"); // Bypass OS security model
            options.AddArgument("--disable-dev-shm-usage"); // Overcome limited resource problems
            options.AddArgument("--disable-extensions"); // Disable extensions
            options.AddArgument("--blink-settings=imagesEnabled=false"); // Disable images
            options.AddArgument("--disable-logging"); // Disable logging
            options.AddArgument("--log-level=3"); // Suppress console logging
            options.AddArgument("--silent"); // Suppress ChromeDriver logs
            options.AddArgument("--enable-unsafe-swiftshader"); // Enable unsafe swiftshader

            using (ChromeDriver driver = new ChromeDriver(options))
            {
                try
                {
                    driver.Navigate().GoToUrl(url);

                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                    string renderedHtml = driver.PageSource;

                    // Regex to find the selectedID in the page HTML
                    var regex = new Regex(@"""selectedID"":""(\d+)""");
                    var match = regex.Match(renderedHtml);

                    if (match.Success)
                    {
                        string pageId = match.Groups[1].Value;
                        Console.WriteLine("Found Page ID: " + pageId);
                        return new PageInfoModel { Id = pageId };
                    }
                    else
                    {
                        Console.WriteLine("ID not found in the page.");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                    return null;
                }
            }
        }

        public override async Task<BaseResponse<BaseResponseModel>> SendSearchRequestAsync(string search, Pagination? nextPage = null)
        {
            try
            {
                // Form data as URL-encoded
                var fbApiReqFriendlyName = "SearchCometResultsPaginatedResultsQuery";
                var cursorValue = Helper.GetCursorValue(nextPage);
                var variables = "{\"allow_streaming\":false,\"args\":{\"callsite\":\"COMET_GLOBAL_SEARCH\",\"config\":{\"exact_match\":false,\"high_confidence_config\":null,\"intercept_config\":null,\"sts_disambiguation\":null,\"watch_config\":null},\"context\":{\"bsid\":\"82938a80-426c-4582-b3ae-68b47d8c05f8\",\"tsid\":null},\"experience\":{\"client_defined_experiences\":[\"ADS_PARALLEL_FETCH\"],\"encoded_server_defined_params\":null,\"fbid\":null,\"type\":\"PAGES_TAB\"},\"filters\":[],\"text\":\"" + search + "\"},\"count\":5,\"cursor\":" + cursorValue + ",\"feedLocation\":\"SEARCH\",\"feedbackSource\":23,\"fetch_filters\":true,\"focusCommentID\":null,\"locale\":null,\"privacySelectorRenderLocation\":\"COMET_STREAM\",\"renderLocation\":\"search_results_page\",\"scale\":1,\"stream_initial_count\":0,\"useDefaultActor\":false,\"__relay_internal__pv__GHLShouldChangeAdIdFieldNamerelayprovider\":false,\"__relay_internal__pv__GHLShouldChangeSponsoredDataFieldNamerelayprovider\":false,\"__relay_internal__pv__IsWorkUserrelayprovider\":false,\"__relay_internal__pv__CometFeedStoryDynamicResolutionPhotoAttachmentRenderer_experimentWidthrelayprovider\":500,\"__relay_internal__pv__CometImmersivePhotoCanUserDisable3DMotionrelayprovider\":false,\"__relay_internal__pv__WorkCometIsEmployeeGKProviderrelayprovider\":false,\"__relay_internal__pv__IsMergQAPollsrelayprovider\":false,\"__relay_internal__pv__FBReelsMediaFooter_comet_enable_reels_ads_gkrelayprovider\":false,\"__relay_internal__pv__CometUFIReactionsEnableShortNamerelayprovider\":false,\"__relay_internal__pv__CometUFIShareActionMigrationrelayprovider\":true,\"__relay_internal__pv__StoriesArmadilloReplyEnabledrelayprovider\":true,\"__relay_internal__pv__EventCometCardImage_prefetchEventImagerelayprovider\":false}";
                var docId = 8928208360561052;
                var extraFormData = new Dictionary<string, string>
                {
                    { "fb_api_req_friendly_name", fbApiReqFriendlyName },
                    { "variables", variables },
                    { "doc_id", docId.ToString() }
                };

                var content = new FormUrlEncodedContent(extraFormData.Concat(_basicFormData));
                var response = await _httpClient.PostAsync(Url, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonObj = await Helper.DeserializeResponseToDynamic(response);
                    var pageMapper = new PageMapper();
                    var pageInfoResults = pageMapper.MapTo(jsonObj);

                    return new BaseResponse<BaseResponseModel>
                    {
                        Models = pageInfoResults.Models.Cast<BaseResponseModel>().ToList(),
                        Pagination = pageInfoResults.Pagination
                    };
                }

                throw new Exception("Request failed with status code: " + response.StatusCode);
            }
            catch (NewDtsgTokenException _)
            {
                Console.WriteLine("New dtsg token exception occurred, retrying the request.");
                return await SendSearchRequestAsync(search, nextPage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during SendSearchRequestAsync for pages: {ex.Message}");
                await TryReLoginAsync();
                return await SendSearchRequestAsync(search, nextPage);
            }
        }

        public override async Task<BaseResponse<FacebookUser>> GetFacebookUsersFor(BaseResponseModel model, Pagination? nextPostPage = null, int limit = 0)
        {
            var totalResults = 0;

            var pageInfoModel = model as PageInfoModel;
            if (pageInfoModel == null)
                return new BaseResponse<FacebookUser>();

            //send request to fetch the next post using next page cursor
            var postResponse = await FetchPostFromPage(pageInfoModel, nextPostPage);
            if (postResponse == null || postResponse.Models == null || !postResponse.Models.Any())
                return new BaseResponse<FacebookUser>();

            var postIntegrationService = new FacebookPostsIntegrationService();

            IEnumerable<FacebookUser> users = new List<FacebookUser>();

            Pagination usersNextPage = null;
            do
            {
                var newUsersResposne = await postIntegrationService.GetFacebookUsersFor(postResponse.Models.First(), usersNextPage);

                users = users.Concat(newUsersResposne.Models);

                usersNextPage = newUsersResposne.Pagination;

                totalResults += newUsersResposne.Models.Count;

            } while (totalResults < limit && usersNextPage.Has_Next_Page);

            return new BaseResponse<FacebookUser>
            {
                Models = users.ToList(),
                Pagination = postResponse.Pagination
            };
        }

        public async Task<BaseResponse<PostInfoModel>> FetchPostFromPage(PageInfoModel pageInfo, Pagination? nextPage = null)
        {
            try
            {
                // Form data as URL-encoded
                var fbApiReqFriendlyName = "ProfileCometTimelineFeedRefetchQuery";
                var cursorValue = Helper.GetCursorValue(nextPage);
                var variables = "{\"afterTime\":null,\"beforeTime\":null,\"count\":3,\"cursor\":" + cursorValue + ",\"feedLocation\":\"TIMELINE\",\"feedbackSource\":0,\"focusCommentID\":null,\"memorializedSplitTimeFilter\":null,\"omitPinnedPost\":true,\"postedBy\":{\"group\":\"OWNER\"},\"privacy\":null,\"privacySelectorRenderLocation\":\"COMET_STREAM\",\"renderLocation\":\"timeline\",\"scale\":1,\"stream_count\":1,\"taggedInOnly\":null,\"trackingCode\":null,\"useDefaultActor\":false,\"id\":\"" + pageInfo.Id + "\",\"__relay_internal__pv__GHLShouldChangeAdIdFieldNamerelayprovider\":false,\"__relay_internal__pv__GHLShouldChangeSponsoredDataFieldNamerelayprovider\":false,\"__relay_internal__pv__IsWorkUserrelayprovider\":false,\"__relay_internal__pv__CometFeedStoryDynamicResolutionPhotoAttachmentRenderer_experimentWidthrelayprovider\":500,\"__relay_internal__pv__CometImmersivePhotoCanUserDisable3DMotionrelayprovider\":false,\"__relay_internal__pv__WorkCometIsEmployeeGKProviderrelayprovider\":false,\"__relay_internal__pv__IsMergQAPollsrelayprovider\":false,\"__relay_internal__pv__FBReelsMediaFooter_comet_enable_reels_ads_gkrelayprovider\":false,\"__relay_internal__pv__CometUFIReactionsEnableShortNamerelayprovider\":false,\"__relay_internal__pv__CometUFIShareActionMigrationrelayprovider\":true,\"__relay_internal__pv__StoriesArmadilloReplyEnabledrelayprovider\":true,\"__relay_internal__pv__EventCometCardImage_prefetchEventImagerelayprovider\":false}";
                var docId = 9278858502175880;
                var extraFormData = new Dictionary<string, string>
                {
                    { "fb_api_req_friendly_name", fbApiReqFriendlyName },
                    { "variables", variables },
                    { "doc_id", docId.ToString() }
                };

                var content = new FormUrlEncodedContent(extraFormData.Concat(_basicFormData));
                var response = await _httpClient.PostAsync(Url, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonObj = await Helper.DeserializeResponseToDynamic(response);
                    var postMapper = new PostFromPageMapper();
                    var postInfoResults = postMapper.MapTo(jsonObj);

                    return new BaseResponse<PostInfoModel>
                    {
                        Models = postInfoResults.Models,
                        Pagination = postInfoResults.Pagination
                    };
                }

                throw new Exception("Request failed with status code: " + response.StatusCode);
            }
            catch (NewDtsgTokenException _)
            {
                Console.WriteLine("New dtsg token exception occurred, retrying the request.");
                return await FetchPostFromPage(pageInfo, nextPage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during Fetching post from page with id ({pageInfo.Id}) : {ex.Message}");
                await TryReLoginAsync();
                return await FetchPostFromPage(pageInfo, nextPage);
            }
        }
    }
}
