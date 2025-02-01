using FacebookAutomation.Contracts.IFacebook;
using FacebookAutomation.Exceptions;
using FacebookAutomation.Mapping;
using FacebookAutomation.Models.Facebook;
using FacebookAutomation.Services.Facebook.Feedback_Algorithms;
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
            FeedbackAlgos =
            [
                new PostReactorsFetchingAlgo(Url, _basicFormData),
                new PostCommentsFetchingAlgo(Url, _basicFormData),
                new PostResharesFetchingAlgo(Url, _basicFormData)
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
                    var postInfoResults = postMapper.MapTo(jsonObj);

                    return new BaseResponse<BaseResponseModel>
                    {
                        Models = postInfoResults.Models.Cast<BaseResponseModel>().ToList(),
                        Pagination = postInfoResults.Pagination
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
                Console.WriteLine($"Error during SendSearchRequestAsync to fetch the next post: {ex.Message}");
                await TryReLoginAsync();
                return await SendSearchRequestAsync(search, nextPage);
            }
        }


        public override async Task<BaseResponse<FacebookUser>> GetFacebookUsersFor(BaseResponseModel model, Pagination? nextPage = null, int limit = 0)
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
            catch (NewDtsgTokenException _)
            {
                Console.WriteLine("New dtsg token exception occurred, retrying the request.");
                return await GetFacebookUsersFor(model, nextPage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during GetFacebookUsersFor post: {ex.Message}");
                await TryReLoginAsync();
                return await GetFacebookUsersFor(model, nextPage);
            }
        }

        public async Task<bool> ReactOnPost(PostInfoModel postInfo, string reaction)
        {
            try
            {
                var fbApiReqFriendlyName = "CometUFIFeedbackReactMutation";
                var variables = "{\"input\":{\"attribution_id_v2\":null,\"feedback_id\":\"" + postInfo.FeedbackId + "\",\"feedback_reaction_id\":\"" + reaction + "\",\"feedback_source\":\"NEWS_FEED\",\"feedback_referrer\":\"/privacy/consent/\",\"is_tracking_encrypted\":true,\"tracking\":null,\"session_id\":null,\"actor_id\":\"" + FormDataState.Instance.GetUserId() + "\",\"client_mutation_id\":\"3\"},\"useDefaultActor\":false,\"__relay_internal__pv__CometUFIReactionsEnableShortNamerelayprovider\":false}";
                var docId = 8995964513767096;
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
                    return true;
                }

                throw new Exception($"Request failed with for post with id ({postInfo.PostId}) status code: " + response.StatusCode);
            }
            catch (NewDtsgTokenException _)
            {
                Console.WriteLine("New dtsg token exception occurred, retrying the request.");
                return await ReactOnPost(postInfo, reaction);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during ReactOnPost: {ex.Message}");
                await TryReLoginAsync();
                return await ReactOnPost(postInfo, reaction);
            }
        }

        public async Task<bool> CommentOnPost(PostInfoModel postInfo, string comment)
        {
            try
            {
                var fbApiReqFriendlyName = "useCometUFICreateCommentMutation";
                var variables = "{\"feedLocation\":\"DEDICATED_COMMENTING_SURFACE\",\"feedbackSource\":110,\"groupID\":null,\"input\":{\"client_mutation_id\":\"1\",\"actor_id\":\"" + FormDataState.Instance.GetUserId() + "\",\"attachments\":null,\"feedback_id\":\"" + postInfo.FeedbackId + "\",\"formatting_style\":null,\"message\":{\"ranges\":[],\"text\":\"" + comment + "\"},\"attribution_id_v2\":\"CometHomeRoot.react,comet.home,unexpected,1738096059595,854487,4748854339,,\",\"vod_video_timestamp\":null,\"is_tracking_encrypted\":true,\"tracking\":[],\"feedback_source\":\"DEDICATED_COMMENTING_SURFACE\",\"idempotence_token\":\"client:" + Guid.NewGuid().ToString() + "\",\"session_id\":null},\"inviteShortLinkKey\":null,\"renderLocation\":null,\"scale\":1,\"useDefaultActor\":false,\"focusCommentID\":null,\"__relay_internal__pv__IsWorkUserrelayprovider\":false}";
                var docId = 8273470932756421;
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
                    return true;
                }

                throw new Exception($"Request failed with for post with id ({postInfo.PostId}) status code: " + response.StatusCode);
            }
            catch (NewDtsgTokenException _)
            {
                Console.WriteLine("New dtsg token exception occurred, retrying the request.");
                return await CommentOnPost(postInfo, comment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during CommentOnPost: {ex.Message}");
                await TryReLoginAsync();
                return await CommentOnPost(postInfo, comment);
            }
        }
    }
}
