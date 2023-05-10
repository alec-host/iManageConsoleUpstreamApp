namespace iManageConsoleUpstreamApp.Api.FolderPoco
{
    public class Folder
    {
        public string? name { get; set; }
        public string? description { get; set; }
        public string? location { get; set; }
        public string? default_security { get; set; } = "inherit";
        public FolderProfile? profile { get; set; }
    }
}
