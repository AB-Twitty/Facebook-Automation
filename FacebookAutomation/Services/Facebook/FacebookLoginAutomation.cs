using FacebookAutomation.Contracts.IProxy;
using FacebookAutomation.Models;
using FacebookAutomation.Models.Cookies;
using FacebookAutomation.Models.Proxy;
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
        private readonly IProxyService _proxyService;
        private const string FacebookLoginUrl = "https://www.facebook.com/login";

        public FacebookLoginAutomation(IProxyService proxyService)
        {
            _proxyService = proxyService;
        }

        public ReadOnlyCollection<OpenQA.Selenium.Cookie> Login(FacebookAuthModel facebookAuth)
        {
            string extensionPath = string.Empty;
            IWebDriver? driver = null;
            IDevTools? devTools = null;
            DevToolsSession? devToolsSession = null;
            NetworkAdapter? network = null;

            try
            {
                var options = ConfigureChromeOptions(facebookAuth.ProxySettings, out extensionPath);
                driver = new ChromeDriver(options);

                devTools = driver as IDevTools;
                devToolsSession = devTools?.GetDevToolsSession();
                network = EnableNetworkMonitoring(devToolsSession);

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                driver.Navigate().GoToUrl("https://www.facebook.com/");
                if (LoadCookies(driver, facebookAuth.CookiesJson))
                {
                    driver.Navigate().GoToUrl("https://www.facebook.com/");
                    if (driver.Url.Contains("https://www.facebook.com/"))
                    {
                        return driver.Manage().Cookies.AllCookies;
                    }
                }

                PerformLogin(driver, wait, facebookAuth);

                var cookies = driver.Manage().Cookies.AllCookies;
                SaveCookies(driver);
                return cookies;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
            finally
            {
                driver?.Quit();
                DisableNetworkMonitoring(devToolsSession, network);

                if (!string.IsNullOrEmpty(extensionPath) && File.Exists(extensionPath))
                {
                    File.Delete(extensionPath);
                }
            }
        }

        private ChromeOptions ConfigureChromeOptions(ProxySettings proxySettings, out string extensionPath)
        {
            var options = new ChromeOptions();
            options.AddArguments("start-maximized", "--incognito");

            string userProfilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google\\Chrome\\User Data\\Profile 1");
            options.AddArguments($"--user-data-dir={userProfilePath}");

            _proxyService.SetUpProxy(options, proxySettings, out extensionPath);

            return options;
        }

        private NetworkAdapter? EnableNetworkMonitoring(DevToolsSession? devToolsSession)
        {
            if (devToolsSession == null) return null;

            var network = devToolsSession.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V132.DevToolsSessionDomains>().Network;
            network.Enable(new OpenQA.Selenium.DevTools.V132.Network.EnableCommandSettings());
            network.RequestWillBeSent += OnRequestWillBeSent;

            return network;
        }

        private void DisableNetworkMonitoring(DevToolsSession? devToolsSession, NetworkAdapter? network)
        {
            if (devToolsSession != null && network != null)
            {
                network.RequestWillBeSent -= OnRequestWillBeSent;
                network.Disable();
            }
        }

        private void PerformLogin(IWebDriver driver, WebDriverWait wait, FacebookAuthModel facebookAuth)
        {
            driver.Navigate().GoToUrl(FacebookLoginUrl);
            HandleCookiesPrompt(driver, wait);

            var emailElement = wait.Until(d => d.FindElement(By.Id("email")));
            SimulateTyping(driver, emailElement, facebookAuth.Email);
            Thread.Sleep(100);

            var passwordElement = wait.Until(d => d.FindElement(By.Id("pass")));
            SimulateTyping(driver, passwordElement, facebookAuth.Password);
            Thread.Sleep(100);

            var loginButton = wait.Until(d => d.FindElement(By.Name("login")));
            SimulateClick(driver, loginButton);

            wait.Until(d => d.Url.Contains("https://www.facebook.com/"));
            Thread.Sleep(200);
        }

        private bool LoadCookies(IWebDriver driver, string cookies)
        {
            try
            {
                var cookiesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FacebookCookies.json");
                if (File.Exists(cookiesPath))
                {
                    var cookiesJson = File.ReadAllText(cookiesPath);
                    var customCookies = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CustomCookie>>(cookiesJson)
                        ?? new List<CustomCookie>();

                    foreach (var customCookie in customCookies)
                    {
                        if (customCookie.Domain.Contains("facebook.com"))
                        {
                            driver.Manage().Cookies.AddCookie(customCookie.ToSeleniumCookie());
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading cookies: " + ex.Message);
            }
            return false;
        }

        private void SaveCookies(IWebDriver driver)
        {
            try
            {
                var cookies = driver.Manage().Cookies.AllCookies;
                var customCookies = cookies.Select(c => new CustomCookie(c)).ToList();
                var cookiesJson = Newtonsoft.Json.JsonConvert.SerializeObject(customCookies);

                // save the cookies to the salesman api

                var cookiesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FacebookCookies.json");
                File.WriteAllText(cookiesPath, cookiesJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving cookies: " + ex.Message);
            }
        }

        private void HandleCookiesPrompt(IWebDriver driver, WebDriverWait wait)
        {
            try
            {
                var declineButton = wait.Until(d => d.FindElement(By.XPath("//div[@aria-label='Decline optional cookies']")));
                declineButton?.Click();
                Thread.Sleep(100);
            }
            catch (WebDriverTimeoutException)
            {
                // Cookies prompt did not appear, continue with the login process
            }
        }

        private void SimulateTyping(IWebDriver driver, IWebElement element, string text)
        {
            var actions = new Actions(driver);
            foreach (char c in text)
            {
                element.SendKeys(c.ToString());
                Thread.Sleep(new Random().Next(1, 3));

                int offsetX = new Random().Next(-2, 2);
                int offsetY = new Random().Next(-2, 2);
                actions.MoveToElement(element, offsetX, offsetY).Perform();
            }
        }

        private void SimulateClick(IWebDriver driver, IWebElement element)
        {
            var actions = new Actions(driver);
            actions.MoveToElement(element).Click().Perform();
            Thread.Sleep(new Random().Next(1, 3));
        }

        private static readonly object lockObject = new object();
        private static bool isRequestProcessed = false;

        private void OnRequestWillBeSent(object sender, RequestWillBeSentEventArgs e)
        {
            if (e.Request.Url.Contains("graphql"))
            {
                lock (lockObject)
                {
                    if (isRequestProcessed) return;

                    var postData = e.Request.PostData;
                    if (postData != null)
                    {
                        var decodedData = HttpUtility.UrlDecode(postData);
                        var formDataPairs = decodedData.Split('&');
                        foreach (var pair in formDataPairs)
                        {
                            var keyValue = pair.Split('=');
                            if (keyValue.Length == 2)
                            {
                                FormDataState.Instance.SetFormData(keyValue[0], keyValue[1]);
                            }
                        }
                    }

                    isRequestProcessed = true;
                }
            }
        }
    }
}