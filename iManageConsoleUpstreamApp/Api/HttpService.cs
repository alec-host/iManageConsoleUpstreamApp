namespace iManageConsoleUpstreamApp.Api
{
    public class HttpService
    {
        readonly IDocumentApiInterface? documentApiInterface;
        readonly IFolderApiInterface? folderApiInterface;
        readonly ITokenInterface? tokenInterface;
        readonly ICustomerDiscoveryInterface? discoveryInterface;

        public HttpService(IDocumentApiInterface documentApiInterface)
        {
            this.documentApiInterface = documentApiInterface;
        }
        public HttpService(IFolderApiInterface folderApiInterface)
        {
            this.folderApiInterface = folderApiInterface;
        }
        public HttpService(ITokenInterface tokenInterface) 
        { 
            this.tokenInterface = tokenInterface;
        }
        public HttpService(ICustomerDiscoveryInterface discoveryInterface) 
        {
            this.discoveryInterface = discoveryInterface;
        }
        public HttpResponseMessage GrantTokenService(string endPoint,dynamic collection) 
        {
            HttpResponseMessage serverResponse = tokenInterface!.GetToken(endPoint,collection);
            return serverResponse;
        }
        public HttpResponseMessage CustomerDiscoveryService(string endPoint) 
        {
            HttpResponseMessage serverResponse = discoveryInterface!.GetCustomerID(endPoint);
            return serverResponse;
        }
        public HttpResponseMessage FileUploadService(string endPoint,dynamic payload,string filePath)
        {
            Console.Out.WriteLine("1. apiInterface: DOCUMENT_UPLOAD_SERVICE");
            HttpResponseMessage serverResponse = documentApiInterface!.HttpPostAttachedFile(endPoint,payload,filePath);

            return serverResponse;
        }
        public HttpResponseMessage CreateFolderService(string endpoint,string payload) 
        {
            HttpResponseMessage serverResponse = folderApiInterface!.HttpPostCreateSubFolder(endpoint, payload);
            return serverResponse;
        }
    }
}
