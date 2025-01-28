using FacebookAutomation.Contracts.IFacebook;
using FacebookAutomation.Models;
using FacebookAutomation.Models.Facebook;
using FacebookAutomation.Utils;

namespace FacebookAutomation.Services.Facebook
{
    public class PostCommentsFetchingAlgo : IPostFeedbackService
    {
        private readonly HttpClient _httpClient;
        private readonly string Url;

        public PostCommentsFetchingAlgo(HttpClient httpClient, string url)
        {
            _httpClient = httpClient;
            Url = url;
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

            var content = new FormUrlEncodedContent(extraformData.Concat(FormDataState.Instance.GetAllFormData()));
            var response = await _httpClient.PostAsync(Url, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonObj = await Helper.DeserializeResponseToDynamic(response);

                var post_comments_mapper = new PostCommentsMapper();
                var postCommentsResult = post_comments_mapper.MapToPostInfoResult(jsonObj);

                return new BaseResponse<FacebookUser>
                {
                    Models = postCommentsResult.CommentsUsers.Cast<FacebookUser>().ToList(),
                    Pagination = postCommentsResult.Pagination
                };
            }
            else
            {
                Console.WriteLine("Request failed with status code: " + response.StatusCode);
            }

            return new BaseResponse<FacebookUser>();
        }
    }
}
