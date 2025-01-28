using FacebookAutomation.Models.Facebook;

namespace FacebookAutomation.Contracts.IFacebook
{
    public interface IFacebookIntegrationService
    {
        Task<BaseResponse<BaseResponseModel>> SendSearchRequestAsync(string search, Pagination? nextPage = null);
        Task<BaseResponse<FacebookUser>> GetFacebookUsersFor(BaseResponseModel model, Pagination? nextPage = null, int limit = 0);
    }
}