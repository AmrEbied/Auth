

using AzureBlobManager.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;

namespace AzureBlobManager.Models
{
    public class UploadViewModel
    {     
        public Guid? DocId { get; set; }
        public string? ParentId { get; set; }
        public MediaTypeEnums? MediaType { get; set; }
        public IFormFile? FileToUpload { get; set; }
    }
}
