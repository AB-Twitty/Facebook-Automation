using FacebookAutomation.Models.Facebook;

namespace FacebookAutomation.Contracts.IFacebook
{
    public interface IFacebookIntegrationService
    {
        Task<BaseResponse<BaseResponseModel>> SendSearchRequestAsync(string search, PageInfo? nextPage = null);
        Task<BaseResponse<FacebookUser>> GetFacebookUsersFor(BaseResponseModel model, PageInfo? nextPage = null);
    }
}