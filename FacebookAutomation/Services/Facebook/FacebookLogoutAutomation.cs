using FacebookAutomation.Utils;

namespace FacebookAutomation.Services.Facebook
{
    public class FacebookLogoutAutomation
    {
        public const string LOGOUT_URL = "https://www.facebook.com/logout.php?button_location=settings&button_name=logout";

        public static async Task Logout()
        {
            try
            {
                var extraFormData = new Dictionary<string, string>
                {
                    { "h", "Afd8h3UHQAyJ0cWJ2pI" },
                    { "ref", "mb" }
                };

                var content = new FormUrlEncodedContent(extraFormData);
                var response = await HttpClientSingleton.Instance.HttpClient.PostAsync(LOGOUT_URL, content);

                Console.WriteLine("Logged out...");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Logout Failed with exceprion: " + ex.Message);
            }
        }
    }
}
