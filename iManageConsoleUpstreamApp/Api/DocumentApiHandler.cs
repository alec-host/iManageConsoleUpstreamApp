using System.Net;

namespace iManageConsoleUpstreamApp.Api
{
    public class DocumentApiHandler : IDocumentApiInterface,IFolderApiInterface
    {
        private string token; 
        public DocumentApiHandler(string token) 
        {
            this.token = token;
        }
        public HttpResponseMessage HttpPostAttachedFile(string endPoint,dynamic payload,string filePath)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post,endPoint);
            request.Headers.Add("X-Auth-Token", token);

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(payload),"profile");
            content.Add(new StreamContent(File.OpenRead(filePath)),"file",filePath);
            request.Content = content;
            var response = client.SendAsync(request);
            return response.GetAwaiter().GetResult();
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