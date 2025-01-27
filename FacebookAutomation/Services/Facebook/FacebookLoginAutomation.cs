using FacebookAutomation.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V132.Network;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Web;

namespace FacebookAutomation.Services.Facebook
{
    public class FacebookLoginAutomation
    {
        private const string FacebookLoginUrl = "https://www.facebook.com/login";
        //private const string Username = "twitty787@outlook.com";
        //private const string Password = "123456789TWITTY#";

        private const string Username = "mohamedabdelghaffar122@outlook.com";
        private const string Password = "123@mohammed";

        public static ReadOnlyCollection<OpenQA.Selenium.Cookie> Login(string country = "Egypt")
        {
            IWebDriver driver = null;
            try
            {
                // Set up Chrome options
                ChromeOptions options = new ChromeOptions();
                options.AddArguments("start-maximized", "--incognito");

                // Initialize ChromeDriver with options
                driver = new ChromeDriver(options);

                // Enable DevTools session
                var devTools = driver as IDevTools;
                var devToolsSession = devTools?.GetDevToolsSession();

                if (devToolsSession != null)
                {
                    // Enable network monitoring
                    var network = devToolsSession.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V132.DevToolsSessionDomains>().Network;
                    network.Enable(new OpenQA.Selenium.DevTools.V132.Network.EnableCommandSettings());

                    // Subscribe to network events
                    network.RequestWillBeSent += OnRequestWillBeSent;
                }

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                // Open Facebook login page
                driver.Navigate().GoToUrl(FacebookLoginUrl);

                // Set geolocation (optional, if needed)
                SetGeolocation(driver, country);

                // Wait for the email field and simulate typing
                var emailElement = wait.Until(d => d.FindElement(By.Id("email")));
                SimulateTyping(driver, emailElement, Username);
                Thread.Sleep(100);

                // Wait for the password field and simulate typing
                var passwordElement = wait.Until(d => d.FindElement(By.Id("pass")));
                SimulateTyping(driver, passwordElement, Password);
                Thread.Sleep(100);

                // Wait for the login button and click it
                var loginButton = wait.Until(d => d.FindElement(By.Name("login")));
                SimulateClick(driver, loginButton);

                // Wait for the login process to complete
                wait.Until(d => d.Url.Contains("https://www.facebook.com/"));
                Thread.Sleep(200);

                // After login, capture the cookies
                var cookies = driver.Manage().Cookies.AllCookies;
                return cookies;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
            finally
            {
                // Ensure the driver is properly disposed
                driver?.Quit();
            }
        }

        // Simulates human-like typing by adding a random delay between each character
        private static void SimulateTyping(IWebDriver driver, IWebElement element, string text)
        {
            var actions = new Actions(driver);

            foreach (char c in text)
            {
                element.SendKeys(c.ToString());
                Thread.Sleep(new Random().Next(1, 3)); // Random delay between keystrokes (simulate human typing speed)

                // Move the cursor slightly within the element's bounds
                int offsetX = new Random().Next(-2, 2);  // Smaller, more controlled offset
                int offsetY = new Random().Next(-2, 2);  // Smaller, more controlled offset

                // Move the cursor within the element itself
                actions.MoveToElement(element, offsetX, offsetY);
                actions.Perform();
            }
        }

        // Simulates clicking on an element with a slight delay
        private static void SimulateClick(IWebDriver driver, IWebElement element)
        {
            var actions = new Actions(driver);
            actions.MoveToElement(element);
            actions.Click().Build().Perform();
            Thread.Sleep(new Random().Next(1, 3));  // Random delay after clicking
        }

        private static void SetGeolocation(IWebDriver driver, string country)
        {
            string script = string.Empty;

            // Define geolocation for Egypt (Cairo) and Lebanon (Beirut)
            if (country == "Egypt")
            {
                // Cairo, Egypt
                script = @"
                                    navigator.geolocation.getCurrentPosition = function(success, error) {
                                        var position = { coords: { latitude: 30.0444, longitude: 31.2357 } };  // Cairo, Egypt
                                        success(position);
                                    };
                                ";
            }
            else if (country == "Lebanon")
            {
                // Beirut, Lebanon
                script = @"
                                    navigator.geolocation.getCurrentPosition = function(success, error) {
                                        var position = { coords: { latitude: 33.8886, longitude: 35.4955 } };  // Beirut, Lebanon
                                        success(position);
                                    };
                                ";
            }
            else if (country == "Russia")
            {
                // Moscow, Russia
                script = @"
                                    navigator.geolocation.getCurrentPosition = function(success, error) {
                                        var position = { coords: { latitude: 55.7558, longitude: 37.6173 } };  // Moscow, Russia
                                        success(position);
                                    };
                                ";
            }

            // Inject the JavaScript into the browser to spoof the geolocation
            if (!string.IsNullOrEmpty(script))
            {
                ((IJavaScriptExecutor)driver).ExecuteScript(script);
                Console.WriteLine($"Geolocation set to {country}.");
            }
        }

        // Callback function to capture network requests
        private static void OnRequestWillBeSent(object sender, RequestWillBeSentEventArgs e)
        {
            // Look for GraphQL requests (these will typically have the word "graphql" in the URL)
            if (e.Request.Url.Contains("graphql"))
            {
                // Optionally, inspect the payload of the GraphQL request here (e.g., POST data)
                var postData = e.Request.PostData;

                if (postData != null)
                {
                    // Decode the form data
                    var decodedData = HttpUtility.UrlDecode(postData);

                    // Split the form data into key-value pairs
                    var formDataPairs = decodedData.Split('&');

                    // Add each key-value pair to the FormDataState
                    foreach (var pair in formDataPairs)
                    {
                        var keyValue = pair.Split('=');
                        if (keyValue.Length == 2)
                        {
                            var key = keyValue[0];
                            var value = keyValue[1];
                            FormDataState.Instance.SetFormData(key, value);
                        }
                    }
                }
            }
        }
    }
}