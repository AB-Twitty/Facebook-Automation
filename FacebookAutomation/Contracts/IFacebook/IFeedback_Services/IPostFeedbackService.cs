using FacebookAutomation.Models.Facebook;

namespace FacebookAutomation.Contracts.IFacebook.IFeedback_Services
{
    public interface IPostFeedbackService
    {
        public Task<BaseResponse<FacebookUser>> GetUsersWithFeedbackOnPost(PostInfoModel postInfo, Pagination? pageInfo = null);
    }
}
