using FacebookAutomation.Models.Facebook;

namespace FacebookAutomation.Contracts.IFacebook.IFeedback_Services
{
    public interface ICommentsService
    {
        public Task<BaseResponse<CommentModel>> GetCommentsForPost(PostInfoModel postInfo, Pagination? pagination = null);
        public Task ReactToComment(CommentModel comment, ReactionsEnum reaction);
        public Task CommentOnComment(CommentModel comment, string commentText);
    }
}
