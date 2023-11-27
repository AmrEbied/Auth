using AzureBlobManager.Models;
using System.Drawing;
using System.Drawing.Imaging;

namespace AzureBlobManager.Infrastructure.Services.Common
{
    public interface ICommonService
    {
        Task<ResponseViewModel> CreateDirectory(UploadViewModel model);
        Task<ImageFormat> GetEncoder(string extension);
        Task<Bitmap> CreateThumbnail(Stream lcFilename, int lnWidth, int lnHeight);
        Task<bool> DeleteDirectory(string path);
    }
}
