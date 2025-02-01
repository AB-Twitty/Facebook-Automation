using FacebookAutomation.Models.Proxy;

namespace FacebookAutomation.Models
{
    public class FacebookAuthModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? CookiesJson { get; set; }
        public ProxySettings ProxySettings { get; set; }
    }
}
