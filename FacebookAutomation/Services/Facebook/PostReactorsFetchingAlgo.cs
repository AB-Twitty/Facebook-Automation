using FacebookAutomation.Contracts.IFacebook;
using FacebookAutomation.Models;
using FacebookAutomation.Models.Facebook;
using FacebookAutomation.Utils;

namespace FacebookAutomation.Services.Facebook
{
    public class PostReactorsFetchingAlgo : IPostFeedbackService
    {
        private readonly HttpClient _httpClient;
        private readonly string Url;

        public PostReactorsFetchingAlgo(HttpClient httpClient, string url)
        {
            _httpClient = httpClient;
            Url = url;
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

            var content = new FormUrlEncodedContent(extraformData.Concat(FormDataState.Instance.GetAllFormData()));
            var response = await _httpClient.PostAsync(Url, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonObj = await Helper.DeserializeResponseToDynamic(response);

                var post_reactions_mapper = new PostReactionsMapper();
                var postReactionsResult = post_reactions_mapper.MapToPostInfoResult(jsonObj);

                return new BaseResponse<FacebookUser>
                {
                    Models = postReactionsResult.Reactors.Cast<FacebookUser>().ToList(),
                    Pagination = postReactionsResult.Pagination
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
