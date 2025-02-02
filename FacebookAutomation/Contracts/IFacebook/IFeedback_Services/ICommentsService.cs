using FacebookAutomation.Models.Facebook;

namespace FacebookAutomation.Contracts.IFacebook.IFeedback_Services
{
    public interface ICommentsService
    {
        public Task<BaseResponse<CommentModel>> GetCommentsForPost(PostInfoModel postInfo, Pagination? pagination = null);
        public Task<bool> CommentOn(string feedbackId, string commentText, bool isCommentReply = false);
    }
}
