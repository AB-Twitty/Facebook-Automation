namespace FacebookAutomation.Utils
{
    public static class ProxyManager
    {
        private static int currentProxyIndex;

        static ProxyManager()
        {
            Random random = new Random();
            currentProxyIndex = random.Next(ProxyList.Count);
        }

        public static (string IP, int Port, string Username, string Password) GetNextProxy()
        {
            var (ip, port, username, password) = ProxyList[currentProxyIndex];
            currentProxyIndex = (currentProxyIndex + 1) % ProxyList.Count;

            Console.WriteLine($"Using proxy: {ip}:{port}");

            return (ip, port, username, password);
        }

        public readonly static List<(string IP, int Port, string Username, string Password)> ProxyList =
        [
            // proxies here
        ];
    }
}
