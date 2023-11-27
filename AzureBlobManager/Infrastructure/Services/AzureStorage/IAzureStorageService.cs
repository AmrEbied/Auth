using AzureBlobManager.Models;
using Microsoft.AspNetCore.Http;

namespace AzureBlobManager.Infrastructure.Services.AzureStorage
{
    public interface IAzureStorageService
    {
        Task<ResponseVM> UploadFile(IFormFile files, string fileName = "", string strDirectoryName = "", string strLocalDirectoryName = "", string extension = "", bool isThumbnail = false);
        Task<DownloadFileVM> DownloadFile(string referenceId);
        Task<ResponseVM> DeleteFile(string referenceId);
    }
}
