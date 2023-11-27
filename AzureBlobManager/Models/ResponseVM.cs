namespace AzureBlobManager.Models
{
    public class ResponseVM
    {
        public string? Message { get; set; }
        public bool? Success { get; set; }  
    }

    public class DownloadFileVM
    {
        public Stream? blobStream { get; set; }
        public string? ContentType { get; set; }
        public string? Name { get; set; }
    }
}
