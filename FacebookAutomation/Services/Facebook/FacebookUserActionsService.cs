using FacebookAutomation.Contracts.IFacebook;
using FacebookAutomation.Utils;

namespace FacebookAutomation.Services.Facebook
{
    public class FacebookUserActionsService : IFacebookUserActionsService
    {
        protected static HttpClient HttpClient => HttpClientSingleton.Instance.HttpClient;
        private readonly Dictionary<string, string> _basicFormData;
        private readonly string Url;

        public FacebookUserActionsService(string url, Dictionary<string, string> basicFormData)
        {
            Url = url;
            _basicFormData = basicFormData;
        }

        public Task<bool> FollowUser(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PokeUser(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task SendBulkFriendRequests(List<string> userIds)
        {
            // Form data as URL-encoded
            var fbApiReqFriendlyName = "FriendingCometFriendRequestSendMutation";

            var stringUserIds = $"[\"{string.Join("\",\"", userIds)}\"]";
            var variables = "{\"input\":{\"attribution_id_v2\":\"ProfileCometTimelineListViewRoot.react,comet.profile.timeline.list,unexpected,1738524720558,613324,190055527696468,,;CometHomeRoot.react,comet.home,via_cold_start,1738524704518,262155,4748854339,,\",\"friend_requestee_ids\":" + stringUserIds + ",\"friending_channel\":\"PROFILE_BUTTON\",\"warn_ack_for_ids\":[],\"actor_id\":\"" + FormDataState.Instance.GetUserId() + "\",\"client_mutation_id\":\"4\"},\"scale\":1}";
            var docId = 9012643805460802;
            var extraFormData = new Dictionary<string, string>
                {
                    { "fb_api_req_friendly_name", fbApiReqFriendlyName },
                    { "variables", variables },
                    { "doc_id", docId.ToString() }
                };

            var content = new FormUrlEncodedContent(extraFormData.Concat(_basicFormData));
            var response = await HttpClient.PostAsync(Url, content);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Friend requests sent successfully");
            else
                Console.WriteLine("Failed to send friend requests");
        }

        public async Task SendFriendRequest(string userId)
        {
            await SendBulkFriendRequests(new List<string> { userId });
        }
    }
}
