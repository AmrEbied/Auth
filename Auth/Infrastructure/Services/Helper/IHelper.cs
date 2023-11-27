

using Auth.Infrastructure.Models;
using AzureBlobManager.Infrastructure.Domain.Entities;

namespace Auth.Infrastructure.Services.Helper
{
    public interface IHelper
    {
        Task<Document> AddFilesByAzureBlob(ListFilesEnumModel listImagesAndEnums,string parentId);
        Task<Document> AddFilesByNormalWay(ListFilesEnumModel listImagesAndEnums,string parentId);
        Task<Document> AddFilesByBase64(ListFilesEnumModel listImagesAndEnums,string parentId);
    

    }
}
