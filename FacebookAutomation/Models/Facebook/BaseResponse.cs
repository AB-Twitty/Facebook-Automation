using System.Text.Json.Serialization;

namespace FacebookAutomation.Models.Facebook
{
    public class BaseResponse<TModel> where TModel : BaseResponseModel
    {
        public Pagination? Pagination { get; set; }
        public IList<TModel>? Models { get; set; }
    }

    public class Pagination
    {
        [JsonPropertyName("end_cursor")]
        public string? End_Cursor { get; set; }

        [JsonPropertyName("has_next_page")]
        public bool? Has_Next_Page { get; set; }
    }

    public interface BaseResponseModel
    {
    }

    public class FacebookUser : BaseResponseModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    public enum FeedbackType
    {
        React,
        Comment,
        Share
    }
}
