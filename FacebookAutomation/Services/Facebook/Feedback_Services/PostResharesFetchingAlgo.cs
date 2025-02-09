﻿using FacebookAutomation.Contracts.IFacebook.IFeedback_Services;
using FacebookAutomation.Mapping;
using FacebookAutomation.Models.Facebook;
using FacebookAutomation.Utils;

namespace FacebookAutomation.Services.Facebook.Feedback_Algorithms
{
    public class PostResharesFetchingAlgo : IPostFeedbackService
    {
        private static HttpClient _httpClient => HttpClientSingleton.Instance.HttpClient;
        private readonly Dictionary<string, string> _baseFormData;
        private readonly string Url;

        public PostResharesFetchingAlgo(string url, Dictionary<string, string> baseFormData)
        {
            Url = url;
            _baseFormData = baseFormData;
        }

        public async Task<BaseResponse<FacebookUser>> GetUsersWithFeedbackOnPost(PostInfoModel postInfo, Pagination? pageInfo = null)
        {
            // Form data as URL-encoded
            var fb_api_req_friendly_name = "CometResharesFeedPaginationQuery";
            var cursorValue = Helper.GetCursorValue(pageInfo);
            var variables = "{\"count\":1,\"cursor\":" + cursorValue + ",\"feedLocation\":\"SHARE_OVERLAY\",\"feedbackSource\":1,\"focusCommentID\":null,\"privacySelectorRenderLocation\":\"COMET_STREAM\",\"renderLocation\":\"reshares_dialog\",\"scale\":1,\"useDefaultActor\":false,\"id\":\"" + postInfo.FeedbackId + "\",\"__relay_internal__pv__IsWorkUserrelayprovider\":false,\"__relay_internal__pv__GHLShouldChangeAdIdFieldNamerelayprovider\":false,\"__relay_internal__pv__GHLShouldChangeSponsoredDataFieldNamerelayprovider\":false,\"__relay_internal__pv__CometFeedStoryDynamicResolutionPhotoAttachmentRenderer_experimentWidthrelayprovider\":500,\"__relay_internal__pv__CometImmersivePhotoCanUserDisable3DMotionrelayprovider\":false,\"__relay_internal__pv__WorkCometIsEmployeeGKProviderrelayprovider\":false,\"__relay_internal__pv__IsMergQAPollsrelayprovider\":false,\"__relay_internal__pv__FBReelsMediaFooter_comet_enable_reels_ads_gkrelayprovider\":false,\"__relay_internal__pv__CometUFIReactionsEnableShortNamerelayprovider\":false,\"__relay_internal__pv__CometUFIShareActionMigrationrelayprovider\":true,\"__relay_internal__pv__StoriesArmadilloReplyEnabledrelayprovider\":true}";
            var doc_id = 8675047469271200;

            var extraformData = new Dictionary<string, string>
            {
                { "fb_api_req_friendly_name", fb_api_req_friendly_name },
                { "variables", variables },
                { "doc_id", doc_id.ToString() }
            };

            var content = new FormUrlEncodedContent(extraformData.Concat(_baseFormData));
            var response = await _httpClient.PostAsync(Url, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonObj = await Helper.DeserializeResponseToDynamic(response);

                var post_reshares_mapper = new PostResharesMapper();
                var postResharesResult = post_reshares_mapper.MapTo(jsonObj);

                return postResharesResult;
            }
            else
            {
                Console.WriteLine("Request failed with status code: " + response.StatusCode);
            }

            return new BaseResponse<FacebookUser>();
        }
    }
}
