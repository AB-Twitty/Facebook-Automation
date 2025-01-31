using FacebookAutomation.Models.Facebook;

namespace FacebookAutomation.Contracts.IMapping
{
    public interface IFacebookResponseMapper<TModel> where TModel : BaseResponseModel
    {
        public BaseResponse<TModel> MapTo(dynamic expandoObject);
    }
}
