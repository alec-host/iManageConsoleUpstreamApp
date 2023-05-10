namespace iManageConsoleUpstreamApp.Api
{
    public class DiscoveryApiHandler : ICustomerDiscoveryInterface
    {
        private string token;
        public DiscoveryApiHandler(string token) 
        {
            this.token = token;
        }
        public HttpResponseMessage GetCustomerID(string endPoint)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get,endPoint);
            request.Headers.Add("X-Auth-Token",token);
            var response = client.SendAsync(request);

            return response.GetAwaiter().GetResult();
        }
    }
}
