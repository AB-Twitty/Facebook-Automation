namespace FacebookAutomation.Models.Facebook
{
    public class CommentModel : BaseResponseModel
    {
        public string? Id { get; set; }
        public string? FeedbackId { get; set; }
        public string? CommentId { get; set; }
        public string? Text { get; set; }
        public string? CommenterId { get; set; }
        public string? CommenterName { get; set; }
        public string? Url { get; set; }
        public DateTime? CommentTime { get; set; }
        public int ReactionsCount { get; set; }
        public int RepliesCount { get; set; }
    }
}
