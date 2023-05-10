namespace iManageConsoleUpstreamApp.Api
{
    public class DocumentApiHandler : IDocumentApiInterface,IFolderApiInterface
    {
        private string token; 
        public DocumentApiHandler(string token) 
        {
            this.token = token;
        }
        public string HttpPostAttachedFile(string endPoint)
        {
            return "UPSTREAM SERVICE 1234";
        }
        public HttpResponseMessage HttpPostCreateSubFolder(string endPoint,string payload)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, endPoint);
            request.Headers.Add("X-Auth-Token", token);
            var content = new StringContent(payload, null, "application/json");
            request.Content = content;
            var response = client.SendAsync(request);
            return response.GetAwaiter().GetResult();
        }
    }
}