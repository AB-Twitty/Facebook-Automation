using FacebookAutomation.Contracts.IFacebook.IFeedback_Services;
using FacebookAutomation.Mapping;
using FacebookAutomation.Models.Facebook;
using FacebookAutomation.Utils;

namespace FacebookAutomation.Services.Facebook.Feedback_Algorithms
{
    public class CommentsService : IPostFeedbackService, ICommentsService
    {
        private static HttpClient _httpClient => HttpClientSingleton.Instance.HttpClient;
        private readonly Dictionary<string, string> _basicFormData;
        private readonly string Url;

        public CommentsService(string url, Dictionary<string, string> basicFormData)
        {
            Url = url;
            _basicFormData = basicFormData;
        }

        private async Task<HttpResponseMessage> SendRequestToFetchCommentsFor(PostInfoModel postInfo, Pagination? pagination = null)
        {
            // Form data as URL-encoded
            var fb_api_req_friendly_name = "CommentsListComponentsPaginationQuery";
            var cursorValue = Helper.GetCursorValue(pagination);
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

            return response;
        }

        public async Task<BaseResponse<FacebookUser>> GetUsersWithFeedbackOnPost(PostInfoModel postInfo, Pagination? pageInfo = null)
        {
            var response = await SendRequestToFetchCommentsFor(postInfo, pageInfo);

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

        public async Task<BaseResponse<CommentModel>> GetCommentsForPost(PostInfoModel postInfo, Pagination? pagination = null)
        {
            var response = await SendRequestToFetchCommentsFor(postInfo, pagination);

            if (response.IsSuccessStatusCode)
            {
                var jsonObj = await Helper.DeserializeResponseToDynamic(response);

                var comments_mapper = new CommentsMapper();
                var commentsResult = comments_mapper.MapTo(jsonObj);

                return commentsResult;
            }
            else
            {
                Console.WriteLine("Request failed with status code: " + response.StatusCode);
            }

            return new BaseResponse<CommentModel>();
        }

        public async Task<bool> CommentOn(string feedbackId, string commentText, bool isCommentReply = false)
        {
            var fbApiReqFriendlyName = "useCometUFICreateCommentMutation";
            var variables = "{\"feedLocation\":\"DEDICATED_COMMENTING_SURFACE\",\"feedbackSource\":110,\"groupID\":null,\"input\":{\"client_mutation_id\":\"1\",\"actor_id\":\"" + FormDataState.Instance.GetUserId() + "\",\"attachments\":null,\"feedback_id\":\"" + feedbackId + "\",\"formatting_style\":null,\"message\":{\"ranges\":[],\"text\":\"" + commentText + "\"},\"attribution_id_v2\":\"CometHomeRoot.react,comet.home,unexpected,1738096059595,854487,4748854339,,\",\"vod_video_timestamp\":null,\"is_tracking_encrypted\":true,\"tracking\":[],\"feedback_source\":\"DEDICATED_COMMENTING_SURFACE\",\"idempotence_token\":\"client:" + Guid.NewGuid().ToString() + "\",\"session_id\":null},\"inviteShortLinkKey\":null,\"renderLocation\":null,\"scale\":1,\"useDefaultActor\":false,\"focusCommentID\":null,\"__relay_internal__pv__IsWorkUserrelayprovider\":false}";
            var docId = isCommentReply ? 8980931231954739 : 8273470932756421;

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
                Console.WriteLine($"Successfully commented on feedback: \"{feedbackId}\"");
                return true;
            }

            Console.WriteLine($"Failed to comment on feedback: \"{feedbackId}\" with status code: {response.StatusCode}");
            return false;
        }
    }
}
