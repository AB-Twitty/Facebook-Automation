namespace FacebookAutomation.Models.Cookies
{
    public class CustomCookie
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
        public DateTime? Expiry { get; set; }
        public bool Secure { get; set; }
        public bool HttpOnly { get; set; }
        public string SameSite { get; set; }

        public CustomCookie()
        {

        }

        public CustomCookie(OpenQA.Selenium.Cookie cookie)
        {
            Name = cookie.Name;
            Value = cookie.Value;
            Domain = cookie.Domain;
            Path = cookie.Path;
            Expiry = cookie.Expiry;
            Secure = cookie.Secure;
            HttpOnly = cookie.IsHttpOnly;
            SameSite = cookie.SameSite;
        }

        public OpenQA.Selenium.Cookie ToSeleniumCookie()
        {
            return new OpenQA.Selenium.Cookie(Name, Value, Domain, Path, Expiry, Secure, HttpOnly, SameSite);
        }
    }
}
