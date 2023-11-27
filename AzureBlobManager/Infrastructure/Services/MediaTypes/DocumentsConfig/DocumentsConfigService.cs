using AzureBlobManager.Infrastructure.Persistence.DataContext;
using AzureBlobManager.Infrastructure.Services.AzureStorage;
using AzureBlobManager.Infrastructure.Services.Common;
using AzureBlobManager.Models;
using Microsoft.EntityFrameworkCore;
using AzureBlobManager.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;

namespace AzureBlobManager.Infrastructure.Services.MediaTypes.DocumentsConfig
{
    public class DocumentsConfigService : IDocumentsConfigService
    {
        private readonly IConfigHelper _configHelper;
        private readonly ICommonService _commonService;
        private readonly IAzureStorageService _azureStorageService;
        public readonly IDocumentDbContext _context;
        public DocumentsConfigService(IDocumentDbContext context, IConfigHelper configHelper, ICommonService commonService, IAzureStorageService azureStorageService)
        {
            _context = context;
            _commonService = commonService;
            _azureStorageService = azureStorageService;
            _configHelper = configHelper;
        }
        public async Task<ResponseViewModel> SaveDocument(UploadViewModel model, CancellationToken cancellationToken = default)
        {
            var existingObj = await _context.Documents?.FirstOrDefaultAsync(p => p.ParentId == model.ParentId && p.MediaTypeId== (int)model.MediaType!)!;
            if (existingObj != null)
            {
                model.DocId = existingObj.DocGuid;
            }
            else
            {
                model.DocId = Guid.NewGuid();
            }
            var IsCreated = await _commonService.CreateDirectory(model);
            if (IsCreated != null && IsCreated.AbsolutePath != null && IsCreated.RootThumbnailsUrl != null && model.FileToUpload!.FileName != null)
            {
                FileInfo fi = new(model.FileToUpload.FileName);
                if (model.FileToUpload == null || model.FileToUpload.Length == 0)
                {
                    return new ResponseViewModel { IsSuccess = false, Errors = "File upload failed!!" };
                }
                var fullFileName = model.DocId.ToString() + fi.Extension;
                if ((await _configHelper.GetConfig("AzureBlob.Media.Image.Extensions")).Contains(fi.Extension.ToLower()))
                {
                    await _azureStorageService.UploadFile(model.FileToUpload, fullFileName, IsCreated.RootThumbnailsUrl, IsCreated.RootThumbnailsLocalUrl!, fi.Extension, true);
                }
                await _azureStorageService.UploadFile(model.FileToUpload, fullFileName, IsCreated.AbsolutePath);

                try
                {
                    if (existingObj != null)
                    {
                        existingObj.FileName = model.FileToUpload.FileName;
                        existingObj.Extension = fi.Extension;
                        existingObj.SizeInByte = model.FileToUpload.Length;
                        _context.Documents?.Update(existingObj);
                    }
                    else
                    {
                        //Domain.Entities.Document doc = new()
                        //{
                        //    Extension = fi.Extension,
                        //    CreatedDate = DateTime.Now,
                        //    ParentId = model.ParentId,
                        //    ParentAlias = "",
                        //    FileName = model.FileToUpload.FileName,
                        //    DocGuid = model.DocId.Value,
                        //    MediaTypeId = (int)model.MediaType!,
                        //    CreatedBy = Guid.NewGuid(),
                        //    SizeInByte = model.FileToUpload.Length,
                        //    FileUrl = IsCreated.AbsolutePath + "/" + fullFileName,
                        //    IsActive = true
                        //};

                        //_context.Documents?.Add(doc);
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                    return new ResponseViewModel { IsSuccess = true, Errors = "File Uploaded Successfully!!", FileExtension = fi.Extension, FilePath = IsCreated.AbsolutePath + "/" + fullFileName, FileName = model.FileToUpload.FileName, SizeInByte = model.FileToUpload.Length };
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return new ResponseViewModel { IsSuccess = true, Errors = "File Uploaded Successfully!!" };
        }
    }
}
