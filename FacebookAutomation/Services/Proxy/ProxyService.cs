using FacebookAutomation.Contracts.IProxy;
using FacebookAutomation.Models.Proxy;
using FacebookAutomation.Utils;
using OpenQA.Selenium.Chrome;
using System.Net;

namespace FacebookAutomation.Services.Proxy
{
    public class ProxyService : IProxyService
    {
        public void SetUpProxy(ChromeOptions options, ProxySettings proxySettings, out string extensionPath)
        {
            //var (ip, port, username, password) = ProxyManager.GetNextProxy();
            extensionPath = CreateProxyAuthExtension(proxySettings);
            options.AddExtension(extensionPath);

            var proxy = new WebProxy($"http://{proxySettings.IpAddress}:{proxySettings.Port}")
            {
                Credentials = new NetworkCredential(proxySettings.Username, proxySettings.Password),
                BypassProxyOnLocal = true
            };

            HttpClientSingleton.Instance.ConfigureProxy(proxy);
        }

        private string CreateProxyAuthExtension(ProxySettings proxySettings)
        {
            string extensionDir = Path.Combine(Path.GetTempPath(), "proxy_auth_extension");
            Directory.CreateDirectory(extensionDir);

            string manifestContent = @"
                {
                    ""version"": ""1.0.0"",
                    ""manifest_version"": 2,
                    ""name"": ""Proxy Auth Extension"",
                    ""permissions"": [
                        ""proxy"",
                        ""tabs"",
                        ""unlimitedStorage"",
                        ""storage"",
                        ""<all_urls>"",
                        ""webRequest"",
                        ""webRequestBlocking""
                    ],
                    ""background"": {
                        ""scripts"": [""background.js""]
                    },
                    ""incognito"": ""split""
                }";

            string backgroundJsContent = $@"
                var config = {{
                    mode: ""fixed_servers"",
                    rules: {{
                        singleProxy: {{
                            scheme: ""http"",
                            host: ""{proxySettings.IpAddress}"",
                            port: parseInt({proxySettings.Port})
                        }},
                        bypassList: [""localhost""]
                    }}
                }};

                chrome.proxy.settings.set({{value: config, scope: ""regular""}}, function() {{}});

                function callbackFn(details) {{
                    return {{
                        authCredentials: {{
                            username: ""{proxySettings.Username}"",
                            password: ""{proxySettings.Password}""
                        }}
                    }};
                }}

                chrome.webRequest.onAuthRequired.addListener(
                    callbackFn,
                    {{urls: [""<all_urls>""]}},
                    ['blocking']
                );";

            File.WriteAllText(Path.Combine(extensionDir, "manifest.json"), manifestContent);
            File.WriteAllText(Path.Combine(extensionDir, "background.js"), backgroundJsContent);

            string zipPath = Path.Combine(Path.GetTempPath(), $"proxy_auth_extension_{Guid.NewGuid()}.zip");
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }

            System.IO.Compression.ZipFile.CreateFromDirectory(extensionDir, zipPath);

            return zipPath;
        }
    }
}