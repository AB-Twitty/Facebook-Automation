using FacebookAutomation.Contracts.IFacebook;
using FacebookAutomation.Models.Facebook;
using FacebookAutomation.Utils;

namespace FacebookAutomation.Services.Facebook
{
    public abstract class FacebookIntegrationService<TModel> : IFacebookIntegrationService where TModel : BaseResponseModel
    {
        protected static HttpClient _httpClient => HttpClientSingleton.Instance.HttpClient;
        protected readonly Dictionary<string, string> _basicFormData;
        protected const string Url = "https://www.facebook.com/api/graphql/";

        public FacebookIntegrationService()
        {
            _basicFormData = FormDataState.Instance.GetAllFormData();
        }

        protected async Task TryReLoginAsync()
        {
            try
            {
                Console.WriteLine("Attempting to re-login...");
                await FacebookLogoutAutomation.Logout();
                //var cookies = FacebookLoginAutomation.Login();
                //HttpClientSingleton.Instance.ConfigureHttpClient(cookies, Url);

                Console.WriteLine("Re-login successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Re-login failed: {ex.Message}. Retrying...");

                await Task.Delay(1000); // delay for 30 seconds
                await TryReLoginAsync();
            }
        }

        // Abstract methods that need to be implemented by subclasses
        public abstract Task<BaseResponse<BaseResponseModel>> SendSearchRequestAsync(string search, Pagination? nextPage = null);
        public abstract Task<BaseResponse<FacebookUser>> GetFacebookUsersFor(BaseResponseModel model, Pagination? nextPage = null, int limit = 0);
    }
}