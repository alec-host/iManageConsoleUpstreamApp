namespace iManageConsoleUpstreamApp.Api
{
    public interface IFolderApiInterface
    {
        HttpResponseMessage HttpPostCreateSubFolder(string endPoint,string payload);

    }
}