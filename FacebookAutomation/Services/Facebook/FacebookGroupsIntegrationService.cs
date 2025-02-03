using FacebookAutomation.Exceptions;
using FacebookAutomation.Mapping;
using FacebookAutomation.Models.Facebook;
using FacebookAutomation.Utils;

namespace FacebookAutomation.Services.Facebook
{
    public class FacebookGroupsIntegrationService : FacebookIntegrationService<GroupInfoModel>
    {
        public FacebookGroupsIntegrationService() : base()
        {
        }

        public override async Task<BaseResponse<BaseResponseModel>> SendSearchRequestAsync(string search, Pagination? nextPage = null)
        {
            try
            {
                // Form data as URL-encoded
                var fbApiReqFriendlyName = "SearchCometResultsPaginatedResultsQuery";
                var cursorValue = Helper.GetCursorValue(nextPage);
                var variables = "{\"allow_streaming\":false,\"args\":{\"callsite\":\"COMET_GLOBAL_SEARCH\",\"config\":{\"exact_match\":false,\"high_confidence_config\":null,\"intercept_config\":null,\"sts_disambiguation\":null,\"watch_config\":null},\"context\":{\"bsid\":null,\"tsid\":null},\"experience\":{\"client_defined_experiences\":[\"ADS_PARALLEL_FETCH\"],\"encoded_server_defined_params\":null,\"fbid\":null,\"type\":\"GROUPS_TAB\"},\"filters\":[],\"text\":\"" + search + "\"},\"count\":15,\"cursor\":" + cursorValue + ",\"feedLocation\":\"SEARCH\",\"feedbackSource\":23,\"fetch_filters\":true,\"focusCommentID\":null,\"locale\":null,\"privacySelectorRenderLocation\":\"COMET_STREAM\",\"renderLocation\":\"search_results_page\",\"scale\":1,\"stream_initial_count\":0,\"useDefaultActor\":false,\"__relay_internal__pv__GHLShouldChangeAdIdFieldNamerelayprovider\":false,\"__relay_internal__pv__GHLShouldChangeSponsoredDataFieldNamerelayprovider\":false,\"__relay_internal__pv__IsWorkUserrelayprovider\":false,\"__relay_internal__pv__CometFeedStoryDynamicResolutionPhotoAttachmentRenderer_experimentWidthrelayprovider\":500,\"__relay_internal__pv__CometImmersivePhotoCanUserDisable3DMotionrelayprovider\":false,\"__relay_internal__pv__WorkCometIsEmployeeGKProviderrelayprovider\":false,\"__relay_internal__pv__IsMergQAPollsrelayprovider\":false,\"__relay_internal__pv__FBReelsMediaFooter_comet_enable_reels_ads_gkrelayprovider\":false,\"__relay_internal__pv__CometUFIReactionsEnableShortNamerelayprovider\":false,\"__relay_internal__pv__CometUFIShareActionMigrationrelayprovider\":true,\"__relay_internal__pv__StoriesArmadilloReplyEnabledrelayprovider\":true,\"__relay_internal__pv__EventCometCardImage_prefetchEventImagerelayprovider\":false}";

                var docId = 29261675110098723;
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
                    var groupMapper = new GroupMapper();
                    var groupInfoResults = groupMapper.MapTo(jsonObj);

                    return new BaseResponse<BaseResponseModel>
                    {
                        Models = groupInfoResults.Models.Cast<BaseResponseModel>().ToList(),
                        Pagination = groupInfoResults.Pagination
                    };
                }

                throw new Exception("Fetching groups failed with status code: " + response.StatusCode);
            }
            catch (NewDtsgTokenException _)
            {
                Console.WriteLine("New dtsg token exception occurred, retrying the request.");
                return await SendSearchRequestAsync(search, nextPage);
            }
        }

        public override Task<BaseResponse<FacebookUser>> GetFacebookUsersFor(BaseResponseModel model, Pagination? nextPage = null, int limit = 0)
        {
            throw new NotImplementedException();
        }
    }
}
