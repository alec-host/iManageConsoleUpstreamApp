namespace iManageConsoleUpstreamApp.Api
{
    public interface IDocumentApiInterface
    {
        HttpResponseMessage HttpPostAttachedFile(string endPoint,dynamic payload,string filePath);
    }
}
