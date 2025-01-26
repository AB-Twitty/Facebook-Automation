using FacebookAutomation.Contracts.IFacebook;
using FacebookAutomation.Models;
using FacebookAutomation.Models.Facebook;
using FacebookAutomation.Utils;

namespace FacebookAutomation.Services.Facebook
{
    public class FacebookPostsIntegrationService : FacebookIntegrationService<PostInfoModel>
    {
        private int NextFeedbackAlgoIdx { get; set; } = 0;
        private IPostFeedbackService? FeedbackAlgo;
        private IPostFeedbackService[] FeedbackAlgos;

        public FacebookPostsIntegrationService() : base()
        {
            SetFeedbackAlgoArray(_httpClient);
        }

        private void SetFeedbackAlgoArray(HttpClient httpClient)
        {
            FeedbackAlgos =
            [
                new PostReactorsFetchingAlgo(httpClient, _basicFormData, Url),
                new PostCommentsFetchingAlgo(httpClient, _basicFormData, Url),
                new PostResharesFetchingAlgo(httpClient, _basicFormData, Url)
            ];
        }

        public override async Task<BaseResponse<BaseResponseModel>> SendSearchRequestAsync(string search, Pagination? nextPage = null)
        {
            try
            {
                // Form data as URL-encoded
                var fbApiReqFriendlyName = "SearchCometResultsPaginatedResultsQuery";
                var cursorValue = Helper.GetCursorValue(nextPage);
                var variables = "{\"allow_streaming\":false,\"args\":{\"callsite\":\"COMET_GLOBAL_SEARCH\",\"config\":{\"exact_match\":false,\"high_confidence_config\":null,\"intercept_config\":null,\"sts_disambiguation\":null,\"watch_config\":null},\"context\":{\"bsid\":\"f806746c-430f-4cdf-a499-48570abe40db\",\"tsid\":\"0.7038002914911556\"},\"experience\":{\"client_defined_experiences\":[\"ADS_PARALLEL_FETCH\"],\"encoded_server_defined_params\":null,\"fbid\":null,\"type\":\"POSTS_TAB\"},\"filters\":[],\"text\":\"" + search + "\"},\"count\":5,\"cursor\":" + cursorValue + ",\"feedLocation\":\"SEARCH\",\"feedbackSource\":23,\"fetch_filters\":true,\"focusCommentID\":null,\"locale\":null,\"privacySelectorRenderLocation\":\"COMET_STREAM\",\"renderLocation\":\"search_results_page\",\"scale\":1,\"stream_initial_count\":0,\"useDefaultActor\":false,\"__relay_internal__pv__GHLShouldChangeAdIdFieldNamerelayprovider\":false,\"__relay_internal__pv__GHLShouldChangeSponsoredDataFieldNamerelayprovider\":false,\"__relay_internal__pv__IsWorkUserrelayprovider\":false,\"__relay_internal__pv__CometFeedStoryDynamicResolutionPhotoAttachmentRenderer_experimentWidthrelayprovider\":500,\"__relay_internal__pv__CometImmersivePhotoCanUserDisable3DMotionrelayprovider\":false,\"__relay_internal__pv__WorkCometIsEmployeeGKProviderrelayprovider\":false,\"__relay_internal__pv__IsMergQAPollsrelayprovider\":false,\"__relay_internal__pv__FBReelsMediaFooter_comet_enable_reels_ads_gkrelayprovider\":false,\"__relay_internal__pv__CometUFIReactionsEnableShortNamerelayprovider\":false,\"__relay_internal__pv__CometUFIShareActionMigrationrelayprovider\":true,\"__relay_internal__pv__StoriesArmadilloReplyEnabledrelayprovider\":true,\"__relay_internal__pv__EventCometCardImage_prefetchEventImagerelayprovider\":false}";

                var docId = 28118757241106260;
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
                    var postMapper = new PostMapper();
                    var postInfoResults = postMapper.MapToPostInfoResult(jsonObj);

                    return new BaseResponse<BaseResponseModel>
                    {
                        Models = postInfoResults.Models.Cast<BaseResponseModel>().ToList(),
                        Pagination = postInfoResults.Pagination
                    };
                }

                throw new Exception("Request failed with status code: " + response.StatusCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during SendSearchRequestAsync to fetch the next post: {ex.Message}");
                await TryReLoginAsync();
                return await SendSearchRequestAsync(search, nextPage);
            }
        }


        public override async Task<BaseResponse<FacebookUser>> GetFacebookUsersFor(BaseResponseModel model, Pagination? nextPage = null)
        {
            try
            {
                FeedbackAlgo ??= FeedbackAlgos[NextFeedbackAlgoIdx++];

                var postInfoModel = model as PostInfoModel;
                if (postInfoModel == null)
                    return new BaseResponse<FacebookUser>();

                var feedbackUsersResponse = await FeedbackAlgo.GetUsersWithFeedbackOnPost(postInfoModel, nextPage);

                if (feedbackUsersResponse.Pagination == null || feedbackUsersResponse.Pagination.Has_Next_Page == false)
                {
                    var nextPageExists = true;
                    if (NextFeedbackAlgoIdx >= FeedbackAlgos.Length)
                    {
                        nextPageExists = false;
                        NextFeedbackAlgoIdx = 0;
                    }

                    FeedbackAlgo = null;
                    feedbackUsersResponse.Pagination = new Pagination
                    {
                        End_Cursor = "",
                        Has_Next_Page = nextPageExists
                    };
                }

                return feedbackUsersResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during GetFacebookUsersFor post: {ex.Message}");
                await TryReLoginAsync();
                return await GetFacebookUsersFor(model, nextPage);
            }
        }
    }
}
