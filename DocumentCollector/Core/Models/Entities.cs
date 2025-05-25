namespace DocumentCollector.Core.Models
{
    public class DataSourceConfig
    {
        public string DataSourceUrl { get; set; }
        public string Token { get; set; }
        public string FolderName { get; set; }
        public string Category { get; set; }
        public string ShortName { get; set; }
    }

    public class Document
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public string Category { get; set; }
        public string ShortName { get; set; }
        public string Content { get; set; }
        public DateTime FetchedAt { get; set; }
        public required string FileName { get; set; }
    }
}
