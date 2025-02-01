using FacebookAutomation.Contracts.IFacebook;
using FacebookAutomation.Mapping;
using FacebookAutomation.Models.Facebook;
using FacebookAutomation.Utils;

namespace FacebookAutomation.Services.Facebook.Feedback_Algorithms
{
    public class PostCommentsFetchingAlgo : IPostFeedbackService
    {
        private static HttpClient _httpClient => HttpClientSingleton.Instance.HttpClient;
        private readonly Dictionary<string, string> _basicFormData;
        private readonly string Url;

        public PostCommentsFetchingAlgo(string url, Dictionary<string, string> basicFormData)
        {
            Url = url;
            _basicFormData = basicFormData;
        }

        public async Task<BaseResponse<FacebookUser>> GetUsersWithFeedbackOnPost(PostInfoModel postInfo, Pagination? pageInfo = null)
        {
            // Form data as URL-encoded
            var fb_api_req_friendly_name = "CommentsListComponentsPaginationQuery";
            var cursorValue = Helper.GetCursorValue(pageInfo);
            var variables = "{\"commentsAfterCount\":null,\"commentsAfterCursor\":" + cursorValue + ",\"commentsBeforeCount\":5,\"commentsBeforeCursor\":null,\"commentsIntentToken\":\"RANKED_UNFILTERED_CHRONOLOGICAL_REPLIES_INTENT_V1\",\"feedLocation\":\"DEDICATED_COMMENTING_SURFACE\",\"focusCommentID\":null,\"scale\":1,\"useDefaultActor\":false,\"id\":\"" + postInfo.FeedbackId + "\",\"__relay_internal__pv__IsWorkUserrelayprovider\":false}";
            var doc_id = 8474861239285785;

            var extraformData = new Dictionary<string, string>
            {
                { "fb_api_req_friendly_name", fb_api_req_friendly_name },
                { "variables", variables },
                { "doc_id", doc_id.ToString() }
            };

            var content = new FormUrlEncodedContent(extraformData.Concat(_basicFormData));
            var response = await _httpClient.PostAsync(Url, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonObj = await Helper.DeserializeResponseToDynamic(response);

                var post_comments_mapper = new PostCommentsMapper();
                var postCommentsResult = post_comments_mapper.MapTo(jsonObj);

                return postCommentsResult;
            }
            else
            {
                Console.WriteLine("Request failed with status code: " + response.StatusCode);
            }

            return new BaseResponse<FacebookUser>();
        }
    }
}
