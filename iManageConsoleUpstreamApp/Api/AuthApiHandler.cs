namespace iManageConsoleUpstreamApp.Api
{
    public class AuthApiHandler : ITokenInterface
    {
        public HttpResponseMessage GetToken(string endPoint,dynamic collection)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post,endPoint);

            var content = new FormUrlEncodedContent(collection);
            request.Content = content;

            var response = client.SendAsync(request);

            return response.GetAwaiter().GetResult();
        }
    }
}