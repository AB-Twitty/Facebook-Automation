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

        public async Task<bool> FollowUser(string userId)
        {
            // Form data as URL-encoded
            var fbApiReqFriendlyName = "CometUserFollowMutation";
            var variables = "{\"input\":{\"attribution_id_v2\":\"ProfileCometTimelineListViewRoot.react,comet.profile.timeline.list,unexpected,1738533814109,371780,190055527696468,,;FriendingCometRoot.react,comet.friending,tap_tabbar,1738533811579,590264,2356318349,,\",\"is_tracking_encrypted\":false,\"subscribe_location\":\"PROFILE\",\"subscribee_id\":\"" + userId + "\",\"tracking\":null,\"actor_id\":\"" + FormDataState.Instance.GetUserId() + "\",\"client_mutation_id\":\"5\"},\"scale\":1}";
            var docId = 28167180839546919;

            var extraFormData = new Dictionary<string, string>
                {
                    { "fb_api_req_friendly_name", fbApiReqFriendlyName },
                    { "variables", variables },
                    { "doc_id", docId.ToString() }
                };

            var content = new FormUrlEncodedContent(extraFormData.Concat(_basicFormData));
            var response = await HttpClient.PostAsync(Url, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonObj = await Helper.DeserializeResponseToDynamic(response);
                var follow_status = jsonObj?["data"]?["actor_subscribe"]?["subscribee"]?["subscribe_status"]?.ToString();

                if (follow_status == "IS_SUBSCRIBED")
                {
                    Console.WriteLine($"Successfully followed user \"{userId}\"");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Failed to follow user \"{userId}\" with status: {follow_status}");
                    return false;
                }
            }
            else
            {
                Console.WriteLine($"Failed to follow user \"{userId}\" with status code: {response.StatusCode}");
                return false;
            }
        }

        public async Task<bool> PokeUser(string userId)
        {
            // Form data as URL-encoded
            var fbApiReqFriendlyName = "PokesMutatorPokeMutation";
            var variables = "{\"input\":{\"client_mutation_id\":\"1\",\"actor_id\":\"" + FormDataState.Instance.GetUserId() + "\",\"user_id\":\"" + userId + "\"}}";

            var docId = 5028133957233114;
            var extraFormData = new Dictionary<string, string>
                {
                    { "fb_api_req_friendly_name", fbApiReqFriendlyName },
                    { "variables", variables },
                    { "doc_id", docId.ToString() }
                };

            var content = new FormUrlEncodedContent(extraFormData.Concat(_basicFormData));
            var response = await HttpClient.PostAsync(Url, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonObj = await Helper.DeserializeResponseToDynamic(response);
                var poke_status = jsonObj?["data"]?["user_poke"]?["user"]?["poke_status"]?.ToString();

                if (poke_status == "PENDING")
                {
                    Console.WriteLine($"Successfully poked user \"{userId}\"");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Failed to poke user \"{userId}\" with status: {poke_status}");
                    return false;
                }
            }
            else
            {
                Console.WriteLine($"Failed to poke user \"{userId}\" with status code: {response.StatusCode}");
                return false;
            }
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
