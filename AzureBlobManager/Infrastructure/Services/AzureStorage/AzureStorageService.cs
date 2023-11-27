using AzureBlobManager.Infrastructure.Helpers;
using AzureBlobManager.Infrastructure.Persistence.DataContext;
using AzureBlobManager.Infrastructure.Services.Common;
using AzureBlobManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Drawing;

namespace AzureBlobManager.Infrastructure.Services.AzureStorage
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly IConfigHelper _configHelper;
        private readonly ICommonService _commonService;
        public readonly IDocumentDbContext _context;
        public AzureStorageService(IConfigHelper configHelper, ICommonService commonService, IDocumentDbContext context)
        {
            _configHelper = configHelper;
            _commonService = commonService;
            _context = context;
        }

        public async Task<ResponseVM> UploadFile(IFormFile files, string fileName = "", string strDirectoryName = "",
            string strLocalDirectoryName = "", string extension = "", bool isThumbnail = false)
        {
            string blobstorageconnection = await _configHelper.GetConfig("AzureBlob.Media.Container.ConnectionString");
            // Retrieve storage account from connection string.
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
            // Create the blob client.
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(await _configHelper.GetConfig("AzureBlob.Media.Container.Name"));
            //Create dynamic directory on azure blob
            CloudBlobDirectory directory = container.GetDirectoryReference(strDirectoryName);
            // This also does not make a service call; it only creates a local object.
            CloudBlockBlob blockBlob = directory.GetBlockBlobReference(fileName);

            await using (var data = files.OpenReadStream())
            {
                //Generate Thumbnail image
                if (isThumbnail)
                {
                    var thumbnailWidth = Convert.ToInt32(await _configHelper.GetConfig("AzureBlob.Media.Thumbnails.Width"));
                    var thumbnailHeight = Convert.ToInt32(await _configHelper.GetConfig("AzureBlob.Media.Thumbnails.Height"));
                    var path = strLocalDirectoryName + "/" + fileName;
                    Bitmap bitmap = await _commonService.CreateThumbnail(data, thumbnailWidth, thumbnailHeight);
                    var encoder = await _commonService.GetEncoder(extension);
                    //file save local
                    bitmap.Save(path, encoder);
                    //Upload on azure 
                    await blockBlob.UploadFromFileAsync(path);
                    //Delete local directory after upload on azure 
                    _commonService.DeleteDirectory(strLocalDirectoryName);
                }
                else
                {
                    await blockBlob.UploadFromStreamAsync(data);
                }
            }
            return new ResponseVM { Message = "File Uploaded Successfully", Success = true };
        }

        public async Task<DownloadFileVM> DownloadFile(string referenceId)
        {
            var existingObj = await _context.Documents?.FirstOrDefaultAsync(p => p.ParentId == referenceId)!;
            if (existingObj != null)
            {
                string rootUrl = await _configHelper.GetConfig("AzureBlob.Media.Container.Root.Url") + "/" + await _configHelper.GetConfig("AzureBlob.Media.Container.Name") + "/" + existingObj.FileUrl;
                CloudBlockBlob blockBlob;
                await using (MemoryStream memoryStream = new MemoryStream())
                {
                    string blobstorageconnection = await _configHelper.GetConfig("AzureBlob.Media.Container.ConnectionString");
                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
                    CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(await _configHelper.GetConfig("AzureBlob.Media.Container.Name"));
                    CloudBlobDirectory directory = cloudBlobContainer.GetDirectoryReference(rootUrl);
                    blockBlob = directory.GetBlockBlobReference(existingObj.DocGuid.ToString() + existingObj.Extension);
                    await blockBlob.DownloadToStreamAsync(memoryStream);
                }
                Stream blobStream = blockBlob.OpenReadAsync().Result;
                return new DownloadFileVM { blobStream = blobStream, ContentType = blockBlob.Properties.ContentType, Name = blockBlob.Name };
            }
            return new DownloadFileVM { blobStream = null, ContentType = "", Name = "" };
        }

        public async Task<ResponseVM> DeleteFile(string referenceId)
        {
            var existingObj = await _context.Documents?.FirstOrDefaultAsync(p => p.ParentId == referenceId)!;
            if (existingObj != null)
            {
                string rootUrl = await _configHelper.GetConfig("AzureBlob.Media.Container.Root.Url") + "/" + await _configHelper.GetConfig("AzureBlob.Media.Container.Name") + "/" + existingObj.FileUrl;
                string blobstorageconnection = await _configHelper.GetConfig("AzureBlob.Media.Container.ConnectionString");
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                string strContainerName = await _configHelper.GetConfig("AzureBlob.Media.Container.Name");
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);
                CloudBlobDirectory directory = cloudBlobContainer.GetDirectoryReference(rootUrl);
                var blob = directory.GetBlobReference(existingObj.DocGuid.ToString() + existingObj.Extension);
                var result = await blob.DeleteIfExistsAsync();
                return new ResponseVM { Message = "File Deleted Successfully", Success = true };
            }
            return new ResponseVM { Message = "Failed", Success = false };
        }


        public async void CreateThumbnail(string referenceId)
        {
            var existingObj = await _context.Documents?.Where(p => p.Extension!.Contains(".jpg,.png") && p.ParentId == referenceId && p.MediaTypeId == 4).ToListAsync()!;
            if (existingObj != null)
            {
                foreach (var item in existingObj)
                {
                    // https://u4sblobstaging.blob.core.windows.net/sachin-test-container/Posts/Images/Thumbs/f5ee9e6a-722a-4f86-9715-421c3446a307.png
                    string strDirectoryName = await _configHelper.GetConfig("AzureBlob.Media.Container.Root.Url") + "/" + await _configHelper.GetConfig("AzureBlob.Media.Container.Name") + "/Posts/Images/Thumbs/" + item.DocGuid.ToString().ToLower() + item.Extension;
                    string rootUrl = await _configHelper.GetConfig("AzureBlob.Media.Container.Root.Url") + "/" + await _configHelper.GetConfig("AzureBlob.Media.Container.Name") + "/" + item.FileUrl;
                    string blobstorageconnection = await _configHelper.GetConfig("AzureBlob.Media.Container.ConnectionString");
                    // Retrieve storage account from connection string.
                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(blobstorageconnection);
                    // Create the blob client.
                    CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
                    // Retrieve a reference to a container.
                    CloudBlobContainer container = blobClient.GetContainerReference(await _configHelper.GetConfig("AzureBlob.Media.Container.Name"));
                    CloudBlobDirectory directory = container.GetDirectoryReference(strDirectoryName);
                    CloudBlockBlob blockBlob = directory.GetBlockBlobReference(item.DocGuid.ToString().ToLower() + item.Extension);

                    var thumbnailWidth = Convert.ToInt32(await _configHelper.GetConfig("AzureBlob.Media.Thumbnails.Width"));
                    var thumbnailHeight = Convert.ToInt32(await _configHelper.GetConfig("AzureBlob.Media.Thumbnails.Height"));
                    var imageBytes = await GetImageAsByteArray(rootUrl);
                    Stream stream = new MemoryStream(imageBytes);
                    Bitmap bitmap = await _commonService.CreateThumbnail(stream, thumbnailWidth, thumbnailHeight);
                    var encoder = await _commonService.GetEncoder(item.Extension!);
                    //file save local
                    bitmap.Save(rootUrl, encoder);
                    //Upload on azure 
                    await blockBlob.UploadFromFileAsync(rootUrl);
                }
            }
        }

        private async Task<byte[]> GetImageAsByteArray(string urlImage)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(urlImage);
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
