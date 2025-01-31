using FacebookAutomation.Contracts.IMapping;
using FacebookAutomation.Models.Facebook;

namespace FacebookAutomation.Mapping
{
    public abstract class FacebookResponseMapper<TModel> : IFacebookResponseMapper<TModel> where TModel : BaseResponseModel
    {
        protected abstract BaseResponse<TModel> MapToModel(dynamic expandoObject);

        public BaseResponse<TModel> MapTo(dynamic expandoObject)
        {
            try
            {
                return MapToModel(expandoObject);
            }
            catch
            {
                return new BaseResponse<TModel>
                {
                    Models = new List<TModel>(),
                    Pagination = new Pagination
                    {
                        End_Cursor = null,
                        Has_Next_Page = false
                    }
                };
            }
        }
    }
}
