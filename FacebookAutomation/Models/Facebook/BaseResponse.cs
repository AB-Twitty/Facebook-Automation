using System.Text.Json.Serialization;

namespace FacebookAutomation.Models.Facebook
{
    public class BaseResponse<TModel> where TModel : BaseResponseModel
    {
        public Pagination Pagination { get; set; }
        public IList<TModel> Models { get; set; }
    }

    public class Pagination
    {
        [JsonPropertyName("end_cursor")]
        public string? End_Cursor { get; set; }

        [JsonPropertyName("has_next_page")]
        public bool Has_Next_Page { get; set; }
    }

    public interface BaseResponseModel
    {
    }

    public class FacebookUser : BaseResponseModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? FriendshipStatus { get; set; }
        public bool CanSendFriendRequest
        {
            get
            {
                if (string.IsNullOrEmpty(FriendshipStatus))
                    return false;

                return FriendshipStatus == "CAN_REQUEST";
            }
        }
        public string? FollowStatus { get; set; }
        public bool CanFollow
        {
            get
            {
                if (string.IsNullOrEmpty(FollowStatus))
                    return false;
                return FollowStatus == "CAN_SUBSCRIBE";
            }
        }
    }

    public enum FeedbackType
    {
        React,
        Comment,
        Share
    }
}
