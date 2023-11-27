using Auth.Infrastructure.Models;
using AzureBlobManager.Infrastructure.Domain.Entities;
using AzureBlobManager.Infrastructure.Helpers;
using AzureBlobManager.Infrastructure.Services.MediaTypes.DocumentsConfig;

namespace Auth.Infrastructure.Services.Helper
{
    public class Helper : IHelper
    {
        private readonly IDocumentsConfigService _docService;
        private readonly IConfigHelper _config;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public Helper(IDocumentsConfigService docService, IConfigHelper config, IWebHostEnvironment webHostEnvironment)
        {
            _docService = docService;
            _config = config;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<Document> AddFilesByAzureBlob(ListFilesEnumModel listImagesAndEnums, string parentId)
        { 
            AzureBlobManager.Models.ResponseViewModel docRes = await _docService.SaveDocument(new AzureBlobManager.Models.UploadViewModel()
            {
                FileToUpload = listImagesAndEnums.File,
                MediaType = listImagesAndEnums.MediaTypeEnum,
                ParentId = parentId,
            });
            string filePath = await _config.GetConfig("AzureBlob.Media.Container.Root.Url") + "/" /*+ await _config.GetConfig("AzureBlob.Media.Container.Name") + "/"*/ + docRes.FilePath;
            return new Document()
            {
                CreatedBy = null,
                CreatedDate = DateTime.Now,
                FileUrl = filePath,
                IsActive = true,
                ParentId = parentId,
                MediaTypeId = (int)listImagesAndEnums.MediaTypeEnum,
                SizeInByte = docRes.SizeInByte ?? 0,
                FileName = docRes.FileName,
                Extension = docRes.FileExtension,
                DocGuid = Guid.NewGuid(),
            }; 
        }

        public async Task<Document> AddFilesByBase64(ListFilesEnumModel listImagesAndEnums, string parentId)
        {
            long fileSizeInBytes = listImagesAndEnums.File.Length; 
            string fileName = listImagesAndEnums.File.FileName; 
            string fileExtension = Path.GetExtension(fileName); 
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            { 
                listImagesAndEnums.File.CopyTo(ms);
                fileBytes = ms.ToArray(); 
            }
            string base64String = Convert.ToBase64String(fileBytes);

            return new Document()
            {
                CreatedBy = null,
                CreatedDate = DateTime.Now,
                FileUrl = base64String,
                IsActive = true,
                ParentId = parentId,
                MediaTypeId = (int)listImagesAndEnums.MediaTypeEnum,
                SizeInByte = fileSizeInBytes,
                FileName = fileName,
                Extension = fileExtension,
                DocGuid = Guid.NewGuid(),
            };

        }

        public async Task<Document> AddFilesByNormalWay(ListFilesEnumModel listImagesAndEnums, string parentId)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", listImagesAndEnums.File.FileName);
            using FileStream fileStream = new(filePath, FileMode.Create);
            listImagesAndEnums.File.CopyTo(fileStream);

            return new Document()
            {
                CreatedBy = null,
                CreatedDate = DateTime.Now,
                FileUrl = filePath,
                IsActive = true,
                ParentId = parentId,
                MediaTypeId = (int)listImagesAndEnums.MediaTypeEnum,
                SizeInByte = null,
                FileName = listImagesAndEnums.File.FileName,
                Extension = null,
                DocGuid = Guid.NewGuid(),
            };
        }
    }

}
