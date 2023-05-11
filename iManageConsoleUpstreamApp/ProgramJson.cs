using Google.Protobuf;
using iManageConsoleUpstreamApp.Api.DocumentPoco;
using iManageConsoleUpstreamApp.Api.FolderPoco;
using iManageConsoleUpstreamApp.Api.Payload;
using Newtonsoft.Json;

namespace iManageConsoleUpstreamApp
{
    public class ProgramJson
    {
        public static string? GetAuthTokenFromResponse(HttpResponseMessage message)
        {
            Console.Out.WriteLine(message.IsSuccessStatusCode);
            if (message.IsSuccessStatusCode == true)
            {
                var json = JsonConvert.DeserializeObject<dynamic>(message.Content!.ReadAsStringAsync().Result);
                return json!.access_token.ToString();
            }
            else 
            {
                Console.Out.WriteLine("simple");
                return null;
            }
        }
        public static string[] GetCustomerIdFromJsonPayload(HttpResponseMessage message)
        {
            string[] myArray;
            var json = JsonConvert.DeserializeObject<dynamic>(message.Content!.ReadAsStringAsync().Result);
            if (json?.ToString().Contains("not-authenticated") == false)
            {
                string customer_id = json!.data.user.customer_id.ToString();
                string libraryName = json!.data.work.libraries[0].alias.ToString();
                myArray = new string[] { customer_id, libraryName };
            }
            else 
            {
                myArray = new string[] { string.Empty, string.Empty };
            }
            return myArray;
        }
        public static string GetParentFolderIdFromResponse(string serverResponse)
        {
            dynamic? json = JsonConvert.DeserializeObject(serverResponse);

            return json!.data.id;
        }
        public static dynamic BuildFolderCreationBodyPayload(string @class, string custom1, string custom2, string custom29, string name)
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
        public static dynamic BuildFileUploadFormPayload(string fileName, string create_date, string edit_date, string filePath)
        {
            //-.method call.
            string fileExtension = ProgramUtility.GetFileExtension(fileName);

            DocProfile document = new DocProfile();
            document.comment = "FiLe-PaTh " + filePath;
            document.name = fileName.Replace(fileExtension, "").Trim();
            document.type = fileExtension.Replace(".", "").ToUpper();
            document.file_create_date = create_date.ToString();
            document.file_edit_date = edit_date.ToString();

            Profile profile = new Profile();
            profile.doc_profile = document;

            Root root = new Root();
            root.profile = profile;
            root.file = filePath;

            var json = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(root));

            return JsonConvert.SerializeObject(json!.profile).ToString();
        }
    }
}
