namespace AzureBlobManager.Models
{
    public class ResponseViewModel
    {
        public bool IsSuccess { get; set; }
        public object? Errors { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public string? FileExtension { get; set; }
        public long? SizeInByte { get; set; }
        public string? RootUrl { get; set; }
        public string? RootThumbnailsUrl { get; set; }
        public string? RootThumbnailsLocalUrl { get; set; }
        public string? AbsolutePath { get; set; }
    }
}
