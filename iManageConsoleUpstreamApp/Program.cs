﻿using iManageConsoleUpstreamApp.Api;
using iManageConsoleUpstreamApp.Api.DocumentPoco;
using iManageConsoleUpstreamApp.Api.FolderPoco;
using iManageConsoleUpstreamApp.Api.Payload;
using iManageConsoleUpstreamApp.Db;
using IronXL;
using Newtonsoft.Json;
using System.Data;
using System.Text.Json.Nodes;
using System.Web;

internal class Program
{
    private const  string BASE_URL = "https://cloudimanage.com";
    private static void Main(string[] args)
    {
        var recordset = DataRepository.GetFolderDocumentRecords();

        while (recordset != null) { 
            //-.method call.
            FileUpstreamOperation();
        }
        //Console.ReadLine();

        Task.Delay(10).Wait();
    }

    private static void FileUpstreamOperation() {

        Document document = new Document();
        document.comment = "";
        document.database = "";
        document.default_security = "admin";
        document.name = "test";
        document.file_create_date = "";
        document.file_edit_date = "";

        Profile profile = new Profile();

        profile.document = document;
        profile.warnings_for_required_and_disabled_fields = true;

        Root root = new Root();

        root.profile = profile;
        root.file = "";

        var obj = JsonConvert.SerializeObject(root);

        Console.WriteLine(obj);

        ITokenInterface tokenInterface = new AuthApiHandler();
        //-.client credentials.
        var collection = new List<KeyValuePair<string, string>>();
        collection.Add(new("username", "wmarita@mkenga.dev.com"));
        collection.Add(new("password", "Mc=\\EGZ$S#764$"));
        collection.Add(new("grant_type", "password"));
        collection.Add(new("client_id", "150b9570-c608-448c-97a3-d53600467095"));
        collection.Add(new("client_secret", "dacd277e-2c9c-4bec-82f7-cb3bf2197d89"));
        //-.grant token url.
        string getTokenEndpoint = BASE_URL+"/auth/oauth2/token";
        //-.invoke grant token api.
        HttpResponseMessage responseMessage = new HttpService(tokenInterface).GrantTokenService(getTokenEndpoint, collection);
        //-.extract token from json.
        string AUTH_TOKEN = getTokenFromJsonPayload(responseMessage);
        Console.WriteLine(AUTH_TOKEN);
        //-.customer discovery.
        ICustomerDiscoveryInterface discoveryInterface = new DiscoveryApiHandler(AUTH_TOKEN);
        string discoveryEndpoint = BASE_URL+"/api";
        HttpResponseMessage discoverRespMessage = new HttpService(discoveryInterface).CustomerDiscoveryService(discoveryEndpoint);
        string[] CUSTOMER_DISCOVERY = getCustomerIdFromJsonPayload(discoverRespMessage);
        Console.WriteLine(CUSTOMER_DISCOVERY[0]);
        //-.upload file.
        IDocumentApiInterface documentApiInterface = new DocumentApiHandler(AUTH_TOKEN);
        Task<string> something = new HttpService(documentApiInterface).FileUploadService(obj);
        Console.WriteLine(">>>>>>>>>> "+something.Result);
        //-.method call.
        CreateSubFolderFileLogic("1011",AUTH_TOKEN, CUSTOMER_DISCOVERY[0], CUSTOMER_DISCOVERY[1]);
    }


    private static void readExcel()
    {
        WorkBook book =  WorkBook.Load("C:\\Users\\admin\\Downloads\\Sample Unstructured Upstream.xlsx");
        WorkSheet sheet = book.DefaultWorkSheet;
        DataSet dataSet = book.ToDataSet(true);
        book.SaveAsJson("C:\\Users\\admin\\Downloads\\Sample.json");
         
        /*
        foreach (var cell in sheet["A2:"]) {
            Console.WriteLine(cell.Value);
        }
        */
        
        foreach (DataTable table in dataSet.Tables)
        {
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    Console.Write(row[i]);
                }
            }
        }

    }

    private static void CreateSubFolderFileLogic(string parentFolderName,string authToken,string customerId,string libraryName) 
    {
        int recordId;
        string createdFolders = String.Empty;
        string _class,custom1,custom2,custom29,parentId,documentPath,tailingPathPart;
        string childFolderId = String.Empty;
        //-.read from db.
        var recordset = DataRepository.GetFolderDocumentRecords();
        //-.convert recodset to json.
        dynamic jsonObject = JsonConvert.DeserializeObject(Utiliy.SqlDataToJson(recordset));

        foreach (var item in jsonObject)
        {
            recordId = item._id;
            documentPath = item.folder_path;
            parentId = item.parent_folder_id;
            _class = item.@class;
            custom1 = item.custom1;
            custom2 = item.custom2;
            custom29 = item.custom29;
            var jsonPayload = String.Empty;

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
                        jsonPayload = BuildFolderCreationBodyPayload(_class, custom1, custom2, custom29, subFolder[i]);
                        string serverResponse = InvokeFolderCreationApi(authToken, customerId, libraryName, parentId, jsonPayload);
                        Console.WriteLine(serverResponse);
                        //-.method call.
                        childFolderId = getParentFolderIdFromResponse(serverResponse);
                        Console.WriteLine(childFolderId);
                        //-.make as processed.
                        DataRepository.FlagRecordAsProcessed(recordId);
                    }
                    else
                    {
                        if (subFolder[i].Contains(".") == false)
                        {
                            jsonPayload = BuildFolderCreationBodyPayload(_class, custom1, custom2, custom29, subFolder[i]);
                            string serverResponse = InvokeFolderCreationApi(authToken, customerId, libraryName, parentId, jsonPayload);
                            Console.WriteLine(serverResponse);
                            //-.method call.
                            childFolderId = getParentFolderIdFromResponse(serverResponse);
                        }
                        else
                        {
                            Console.WriteLine("<THIS IS OUR FILE> " + subFolder[i]);
                        }
                        //-----.Console.WriteLine("---> " + subFolder[i]+"   ddd  "+childFolderId +"  0000  "+ subFolder[i]);
                        //-.make as processed.
                        DataRepository.FlagRecordAsProcessed(recordId);
                    }
                    createdFolders += backslash + subFolder[i];
                }
                //----.Console.WriteLine(documentPath);
                Task.Delay(1000);
                //System.Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("PARENT FOLDER NOT FOUND.");
                Task.Delay(1000);
                //System.Environment.Exit(0);
            }
        }
    }

    private static void createSubFolder_test(string parentFolderName)
    {
        string listOfFolders = String.Empty;
        //string documentPath = "C:\\Users\\wycli\\Downloads\\Alan A Houston Esq\\Alan A Houston Esq\\1011\\009\\Email\\A Cowboy Says it Best.msg";
        //string tailingPathPart = documentPath.Substring(documentPath.IndexOf(parentFolderName) + parentFolderName.Length);

        //string[] subFolder = tailingPathPart.Split("\\");


        /*
        for (int i=1;i< subFolder.Length-1;i++) {
            Console.WriteLine("---> " + subFolder[i]);
            listOfFolders += "\\"+subFolder[i];
        }
        
        Console.WriteLine(tailingPathPart);
        */
        //Console.WriteLine(listOfFolders);
    }
    private static string InvokeFolderCreationApi(string token,string customerId,string libraryName,string parent_folder_id,dynamic payload) 
    {
        IFolderApiInterface folderApiInterface = new DocumentApiHandler(token);
        string createFolderEndpoint = BASE_URL + "/api/v2/customers/{0}/libraries/{1}/folders/{2}/subfolders".Replace("{0}", customerId).Replace("{1}",libraryName).Replace("{2}",parent_folder_id);
        HttpResponseMessage response = new HttpService(folderApiInterface).CreateFolderService(createFolderEndpoint,payload);
        return response.Content.ReadAsStringAsync().Result;
    }
    private static string getTokenFromJsonPayload(HttpResponseMessage message) 
    {
        var json = JsonConvert.DeserializeObject<dynamic>(message.Content!.ReadAsStringAsync().Result);
        return json!.access_token.ToString();
    }
    private static string[] getCustomerIdFromJsonPayload(HttpResponseMessage message) 
    {
        string[] myArray;
        var json = JsonConvert.DeserializeObject<dynamic>(message.Content!.ReadAsStringAsync().Result);
        string customer_id = json!.data.user.customer_id.ToString();
        string libraryName = json!.data.work.libraries[0].alias.ToString();
        myArray = new string[] {customer_id,libraryName};
        return myArray;
    }
    private static string getParentFolderIdFromResponse(string serverResponse) 
    {
        dynamic? json = JsonConvert.DeserializeObject(serverResponse);

        return json!.data.id;
    }
    private static dynamic BuildFolderCreationBodyPayload(string @class,string custom1,string custom2,string custom29,string name) 
    {
        FolderProfile profile = new FolderProfile();

        profile.@class = @class;
        profile.custom1 = custom1;
        profile.custom2 = custom2;
        profile.custom29 = custom29;

        Folder folder = new Folder();

        folder.name = name;
        folder.description = "Created using a c# solution.";
        folder.location = name;
        folder.profile = profile;

        var json = JsonConvert.SerializeObject(folder);

        return json;
    }
}