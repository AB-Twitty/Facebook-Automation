using FacebookAutomation.Models.Proxy;
using OpenQA.Selenium.Chrome;

namespace FacebookAutomation.Contracts.IProxy
{
    public interface IProxyService
    {
        public void SetUpProxy(ChromeOptions options, ProxySettings proxySettings, out string extensionPath);
    }
}
