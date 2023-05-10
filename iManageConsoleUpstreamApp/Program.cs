using iManageConsoleUpstreamApp;
using iManageConsoleUpstreamApp.Api;
using iManageConsoleUpstreamApp.Db;
using IronXL;
using Newtonsoft.Json;
using System.Data;

internal class Program
{
    private const string BASE_URL = "https://cloudimanage.com";
    private static void Main(string[] args)
    {
        var recordset = DataRepository.GetFolderDocumentRecords();

        //while (recordset != null) { 
        //-.method call.
        FileUpstreamOperation();
        //};

        Task.Delay(10).Wait();
    }
    private static void FileUpstreamOperation()
    {
        ITokenInterface tokenInterface = new AuthApiHandler();
        //-.client credentials.
        var collection = new List<KeyValuePair<string, string>>();
        collection.Add(new("username", "wmarita@mkenga.dev.com"));
        collection.Add(new("password", "Mc=\\EGZ$S#764$"));
        collection.Add(new("grant_type", "password"));
        collection.Add(new("client_id", "150b9570-c608-448c-97a3-d53600467095"));
        collection.Add(new("client_secret", "dacd277e-2c9c-4bec-82f7-cb3bf2197d89"));
        //-.grant token url.
        string getTokenEndpoint = BASE_URL + "/auth/oauth2/token";
        //-.invoke grant token api.
        HttpResponseMessage responseMessage = new HttpService(tokenInterface).GrantTokenService(getTokenEndpoint, collection);
        //-.extract token from json.
        string AUTH_TOKEN = ProgramJson.getTokenFromJsonPayload(responseMessage);
        Console.WriteLine(AUTH_TOKEN);
        //-.customer discovery.
        ICustomerDiscoveryInterface discoveryInterface = new DiscoveryApiHandler(AUTH_TOKEN);
        string discoveryEndpoint = BASE_URL + "/api";
        HttpResponseMessage discoverRespMessage = new HttpService(discoveryInterface).CustomerDiscoveryService(discoveryEndpoint);
        string[] CUSTOMER_DISCOVERY = ProgramJson.getCustomerIdFromJsonPayload(discoverRespMessage);
        //-.method call.
        CreateSubFolderFileLogic("1011", AUTH_TOKEN, CUSTOMER_DISCOVERY[0], CUSTOMER_DISCOVERY[1]);
    }
    private static void CreateSubFolderFileLogic(string parentFolderName, string authToken, string customerId, string libraryName)
    {
        int recordId;
        string file = String.Empty;
        string createdFolders = String.Empty;
        string _class = String.Empty;
        string custom1 = String.Empty;
        string custom2 = String.Empty;
        string custom29 = String.Empty;
        string parentId = String.Empty;
        string documentPath = String.Empty;
        string tailingPathPart = String.Empty;
        string childFolderId = String.Empty;
        string createDate = String.Empty;
        string editDate = String.Empty;
        //-.read from db.
        var recordset = DataRepository.GetFolderDocumentRecords();
        //-.convert recodset to json.
        dynamic jsonObject = JsonConvert.DeserializeObject(Utiliy.SqlDataToJson(recordset));

        foreach (var item in jsonObject)
        {
            recordId = item._id;
            file = item.file;
            documentPath = item.folder_path;
            parentId = item.parent_folder_id;
            _class = item.@class;
            custom1 = item.custom1;
            custom2 = item.custom2;
            custom29 = item.custom29;
            createDate = item.create_date;
            editDate = item.edit_date;

            if (documentPath.Contains(parentFolderName) == true)
            {
                string backslash = @"\";

                tailingPathPart = documentPath.Substring(documentPath.IndexOf(parentFolderName) + parentFolderName.Length);
                string[] subFolder = tailingPathPart.Split(backslash);

                Console.WriteLine("<LIST OF FOLDERS,FILE> " + tailingPathPart);

                for (int i = 1; i < subFolder.Length; i++)
                {
                    if (i == 1)
                    {
                        //------.Console.WriteLine("<start> " + parentId + " " + subFolder[i]);
                        //------.Console.WriteLine("<LOOP_LOGIC> " + custom2);
                        dynamic createFolderPayload = ProgramJson.BuildFolderCreationBodyPayload(_class, custom1, custom2, custom29, subFolder[i]);
                        string serverResponse = InvokeFolderCreationApi(authToken, customerId, libraryName, parentId, createFolderPayload);
                        Console.WriteLine(serverResponse);
                        //-.method call.
                        childFolderId = ProgramJson.getParentFolderIdFromResponse(serverResponse);
                        Console.WriteLine(childFolderId);
                        //-.make as processed.
                        DataRepository.FlagRecordAsProcessed(recordId);
                    }
                    else
                    {
                        if (subFolder[i].Contains(".") == false)
                        {
                            dynamic createFolderPayload = ProgramJson.BuildFolderCreationBodyPayload(_class, custom1, custom2, custom29, subFolder[i]);
                            string serverResponse = InvokeFolderCreationApi(authToken, customerId, libraryName, parentId, createFolderPayload);
                            Console.WriteLine(serverResponse);
                            //-.method call.
                            childFolderId = ProgramJson.getParentFolderIdFromResponse(serverResponse);
                        }
                        else
                        {
                            createDate = ProgramUtility.FormatDateTime(createDate);
                            editDate = ProgramUtility.FormatDateTime(editDate);
                            Console.WriteLine("<THIS IS OUR FILE> " + subFolder[i]);
                            dynamic createFilePayload = ProgramJson.BuildFileUploadFormPayload(file, createDate, editDate, documentPath);
                            Console.WriteLine(createFilePayload);
                            //-.method call.
                            string serverResponse = InvokeFileUploadApi(authToken, customerId, libraryName, childFolderId, createFilePayload, documentPath);
                            Console.WriteLine(serverResponse);
                        }
                        //-----.Console.WriteLine("---> " + subFolder[i]+"   ddd  "+childFolderId +"  0000  "+ subFolder[i]);
                        //-.make as processed.
                        DataRepository.FlagRecordAsProcessed(recordId);
                    }
                    createdFolders += backslash + subFolder[i];
                }
                Task.Delay(100);
            }
            else
            {
                Console.WriteLine("PARENT FOLDER NOT FOUND.");
                Task.Delay(100);
            }
        }
    }
    private static string InvokeFolderCreationApi(string token, string customerId, string libraryName, string parentFolderId, dynamic payload)
    {
        IFolderApiInterface folderApiInterface = new DocumentApiHandler(token);
        string createFolderEndpoint = BASE_URL + "/api/v2/customers/{0}/libraries/{1}/folders/{2}/subfolders".Replace("{0}", customerId).Replace("{1}", libraryName).Replace("{2}", parentFolderId);
        HttpResponseMessage response = new HttpService(folderApiInterface).CreateFolderService(createFolderEndpoint, payload);
        return response.Content.ReadAsStringAsync().Result;
    }
    private static string InvokeFileUploadApi(string token, string customerId, string libraryName, string folderId, dynamic payload, string filePath)
    {
        IDocumentApiInterface documentApiInterface = new DocumentApiHandler(token);
        string getFileUploadEndpoint = BASE_URL + "/api/v2/customers/{0}/libraries/{1}/folders/{2}/documents".Replace("{0}", customerId).Replace("{1}", libraryName).Replace("{2}", folderId);
        HttpResponseMessage response = new HttpService(documentApiInterface).FileUploadService(getFileUploadEndpoint, payload, filePath);
        return response.Content.ReadAsStringAsync().Result;
    }
}