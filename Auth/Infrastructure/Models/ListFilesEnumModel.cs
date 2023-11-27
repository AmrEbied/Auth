using AzureBlobManager.Infrastructure.Helpers;

namespace Auth.Infrastructure.Models
{
    public class ListFilesEnumModel
    {
        public IFormFile? File { get; set; }
        public MediaTypeEnums MediaTypeEnum { get; set; }
    }
}
