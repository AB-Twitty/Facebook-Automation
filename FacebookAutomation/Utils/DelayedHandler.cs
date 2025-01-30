namespace FacebookAutomation.Utils
{
    public class DelayedHandler : DelegatingHandler
    {
        private readonly int _delayMilliseconds;

        public DelayedHandler(HttpMessageHandler innerHandler, int delayMilliseconds) : base(innerHandler)
        {
            _delayMilliseconds = delayMilliseconds;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Add delay before sending the request
            await Task.Delay(_delayMilliseconds, cancellationToken);

            var response = await base.SendAsync(request, cancellationToken);

            // await Task.Delay(_delayMilliseconds, cancellationToken);

            return response;
        }
    }
}
