using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace FacebookAutomation.Services.Facebook
{
    public class FacebookLoginAutomation
    {
        private const string FacebookLoginUrl = "https://www.facebook.com/login";
        private const string Username = "############@####.com";
        private const string Password = "*************";

        public static ReadOnlyCollection<Cookie> Login(string country = "Egypt")
        {
            IWebDriver driver = null;
            try
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArguments("start-maximized");
                options.AddArguments("--incognito");
                //options.AddArguments("--headless"); // Uncomment to run in headless mode (no UI)
                //options.AddArguments("--disable-gpu");

                // Initialize the WebDriver (Chrome in this case, can be changed)
                driver = new ChromeDriver(options);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10)); // Waits up to 10 seconds for elements to appear

                // Open Facebook login page
                driver.Navigate().GoToUrl(FacebookLoginUrl);

                // Set geolocation before interacting with the page
                SetGeolocation(driver, country);

                // Wait for email field to be visible and simulate typing
                var emailElement = wait.Until(d => d.FindElement(By.Id("email")));
                SimulateTyping(driver, emailElement, Username);
                Thread.Sleep(100);

                // Wait for password field to be visible and simulate typing
                var passwordElement = wait.Until(d => d.FindElement(By.Id("pass")));
                SimulateTyping(driver, passwordElement, Password);
                Thread.Sleep(100);

                // Wait for login button and click it
                var loginButton = wait.Until(d => d.FindElement(By.Name("login")));
                SimulateClick(driver, loginButton);

                // Wait for the login process to complete
                wait.Until(d => d.Url.Contains("https://www.facebook.com/"));
                // An additional delay to ensure cookies are fully received and the page is completely loaded
                Thread.Sleep(200);

                // After login, extract cookies
                var cookies = driver.Manage().Cookies.AllCookies;

                return cookies;
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine("Error: Element not found - " + ex.Message);
                throw ex;
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine("Error: Timeout - " + ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error: " + ex.Message);
                throw ex;
            }
            finally
            {
                // Ensure the driver is properly disposed of, even in case of errors
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
    }
}
