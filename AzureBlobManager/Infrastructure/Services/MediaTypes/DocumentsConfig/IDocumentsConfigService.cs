using AzureBlobManager.Models;

namespace AzureBlobManager.Infrastructure.Services.MediaTypes.DocumentsConfig
{
    public interface IDocumentsConfigService
    {
        Task<ResponseViewModel> SaveDocument(UploadViewModel model, CancellationToken cancellationToken = default);
    }
}
