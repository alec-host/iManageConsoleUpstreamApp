using iManageConsoleUpstreamApp;
using iManageConsoleUpstreamApp.Api;
using iManageConsoleUpstreamApp.Db;
using Newtonsoft.Json;

internal class Program
{
    private const string BASE_URL = "https://cloudimanage.com";
    private static void Main(string[] args)
    {
        var recordset = DataRepository.GetFolderDocumentRecords();

        //while (recordset != null) { 
        //-.method call.
        UpstreamOperation(recordset);
        //};

        Task.Delay(10).Wait();
    }
    private static void UpstreamOperation(dynamic recordset)
    {
        string? AUTH_TOKEN = ProgramJson.GetAuthTokenFromResponse(GrantAuthTokenRequest());
        if (AUTH_TOKEN is not null)
        {
            //-.customer discovery.
            ICustomerDiscoveryInterface discoveryInterface = new DiscoveryApiHandler(AUTH_TOKEN);
            string discoveryEndpoint = BASE_URL + "/api";
            HttpResponseMessage discoverRespMessage = new HttpService(discoveryInterface).CustomerDiscoveryService(discoveryEndpoint);
            string[] CUSTOMER_DISCOVERY = ProgramJson.GetCustomerIdFromJsonPayload(discoverRespMessage);
            if (CUSTOMER_DISCOVERY[0].Trim().Length != 0 && CUSTOMER_DISCOVERY[1].Trim().Length != 0)
            {
                BusinessLogic("1011",AUTH_TOKEN,CUSTOMER_DISCOVERY[0],CUSTOMER_DISCOVERY[1],recordset);
            }
            else 
            {
                Console.WriteLine("CUSTOMER DISCOVERY HAS FAILED.");
            }
        }
        else 
        {
            Console.WriteLine("INVALID CLIENT CREDENTIALS PROVIDED.");
        }
    }
    private static void BusinessLogic(string parentFolderName,string authToken,string customerId,string libraryName,dynamic recordset)
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
        
        //var recordset = DataRepository.GetFolderDocumentRecords();
        //-.convert recodset to json.
        dynamic dataObject = JsonConvert.DeserializeObject(Utiliy.SqlDataToJson(recordset));

        foreach (var item in dataObject)
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
                        string serverResponse = FolderCreateRequest(authToken, customerId, libraryName, parentId, createFolderPayload);
                        Console.WriteLine(serverResponse);
                        //-.method call.
                        childFolderId = ProgramJson.GetParentFolderIdFromResponse(serverResponse);
                        Console.WriteLine(childFolderId);
                        //-.make as processed.
                        DataRepository.FlagRecordAsProcessed(recordId);
                    }
                    else
                    {
                        if (subFolder[i].Contains(".") == false)
                        {
                            dynamic createFolderPayload = ProgramJson.BuildFolderCreationBodyPayload(_class, custom1, custom2, custom29, subFolder[i]);
                            string serverResponse = FolderCreateRequest(authToken, customerId, libraryName, parentId, createFolderPayload);
                            Console.WriteLine(serverResponse);
                            //-.method call.
                            childFolderId = ProgramJson.GetParentFolderIdFromResponse(serverResponse);
                        }
                        else
                        {
                            createDate = ProgramUtility.FormatDateTime(createDate);
                            editDate = ProgramUtility.FormatDateTime(editDate);
                            Console.WriteLine("<THIS IS OUR FILE> " + subFolder[i]);
                            dynamic createFilePayload = ProgramJson.BuildFileUploadFormPayload(file, createDate, editDate, documentPath);
                            Console.WriteLine(createFilePayload);
                            //-.method call.
                            string serverResponse = FileUploadRequest(authToken, customerId, libraryName, childFolderId, createFilePayload, documentPath);
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
    private static HttpResponseMessage GrantAuthTokenRequest()
    {
        //-.client credentials.
        var collection = new List<KeyValuePair<string, string>>();
        collection.Add(new("username", "MY_EMAIL"));
        collection.Add(new("password", "MY_PASSWORD"));
        collection.Add(new("grant_type", "password"));
        collection.Add(new("client_id", "MY_KEY"));
        collection.Add(new("client_secret", "MY_SECRET"));

        ITokenInterface tokenInterface = new AuthApiHandler();
        string getTokenEndpoint = BASE_URL + "/auth/oauth2/token";
        HttpResponseMessage responseMessage = new HttpService(tokenInterface).GrantTokenService(getTokenEndpoint, collection);
        return responseMessage;
    }
    private static string FolderCreateRequest(string token, string customerId, string libraryName, string parentFolderId, dynamic payload)
    {
        IFolderApiInterface folderApiInterface = new DocumentApiHandler(token);
        string createFolderEndpoint = BASE_URL + "/api/v2/customers/{0}/libraries/{1}/folders/{2}/subfolders".Replace("{0}", customerId).Replace("{1}", libraryName).Replace("{2}", parentFolderId);
        HttpResponseMessage response = new HttpService(folderApiInterface).CreateFolderService(createFolderEndpoint, payload);
        return response.Content.ReadAsStringAsync().Result;
    }
    private static string FileUploadRequest(string token, string customerId, string libraryName, string folderId, dynamic payload, string filePath)
    {
        IDocumentApiInterface documentApiInterface = new DocumentApiHandler(token);
        string getFileUploadEndpoint = BASE_URL + "/api/v2/customers/{0}/libraries/{1}/folders/{2}/documents".Replace("{0}", customerId).Replace("{1}", libraryName).Replace("{2}", folderId);
        HttpResponseMessage response = new HttpService(documentApiInterface).FileUploadService(getFileUploadEndpoint, payload, filePath);
        return response.Content.ReadAsStringAsync().Result;
    }
}