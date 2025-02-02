using FacebookAutomation.Contracts.IFacebook.IFeedback_Services;
using FacebookAutomation.Mapping;
using FacebookAutomation.Models.Facebook;
using FacebookAutomation.Utils;

namespace FacebookAutomation.Services.Facebook.Feedback_Algorithms
{
    public class ReactionsService : IPostFeedbackService, IReactionsService
    {
        private static HttpClient _httpClient => HttpClientSingleton.Instance.HttpClient;
        private readonly Dictionary<string, string> _baseFormData;
        private readonly string Url;

        public ReactionsService(string url, Dictionary<string, string> baseFormData)
        {
            Url = url;
            _baseFormData = baseFormData;
        }

        public async Task<BaseResponse<FacebookUser>> GetUsersWithFeedbackOnPost(PostInfoModel postInfo, Pagination? pageInfo = null)
        {
            // Form data as URL-encoded
            var fb_api_req_friendly_name = "CometUFIReactionsDialogTabContentRefetchQuery";
            var cursorValue = Helper.GetCursorValue(pageInfo);
            var variables = "{\"count\":15,\"cursor\":" + cursorValue + ",\"feedbackTargetID\":" + $"\"{postInfo.FeedbackId}\"" + ",\"id\":" + $"\"{postInfo.FeedbackId}\"" + "}";
            var doc_id = 9069004929826853;

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

                var post_reactions_mapper = new PostReactionsMapper();
                var postReactionsResult = post_reactions_mapper.MapTo(jsonObj);

                return postReactionsResult;
            }
            else
            {
                Console.WriteLine("Request failed with status code: " + response.StatusCode);
            }

            return new BaseResponse<FacebookUser>();
        }

        public async Task<bool> ReactTo(string feedbackId, ReactionsEnum reactType)
        {
            var reaction = Reactions.GetReactionId(reactType);

            if (string.IsNullOrEmpty(reaction))
                return false;

            var fbApiReqFriendlyName = "CometUFIFeedbackReactMutation";
            var variables = "{\"input\":{\"attribution_id_v2\":null,\"feedback_id\":\"" + feedbackId + "\",\"feedback_reaction_id\":\"" + reaction + "\",\"feedback_source\":\"NEWS_FEED\",\"feedback_referrer\":\"/privacy/consent/\",\"is_tracking_encrypted\":true,\"tracking\":null,\"session_id\":null,\"actor_id\":\"" + FormDataState.Instance.GetUserId() + "\",\"client_mutation_id\":\"3\"},\"useDefaultActor\":false,\"__relay_internal__pv__CometUFIReactionsEnableShortNamerelayprovider\":false}";
            var docId = 8995964513767096;
            var extraFormData = new Dictionary<string, string>
                {
                    { "fb_api_req_friendly_name", fbApiReqFriendlyName },
                    { "variables", variables },
                    { "doc_id", docId.ToString() }
                };
            var content = new FormUrlEncodedContent(extraFormData.Concat(_baseFormData));
            var response = await _httpClient.PostAsync(Url, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Successfully reacted for feedback \"{feedbackId}\"");
                return true;
            }

            Console.WriteLine($"Failed to react for feedback \"{feedbackId}\" with status code: {response.StatusCode}");
            return false;
        }
    }
}
