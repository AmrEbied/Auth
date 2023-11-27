using AzureBlobManager.Infrastructure.Helpers;
using AzureBlobManager.Infrastructure.Persistence.DataContext;
using AzureBlobManager.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace AzureBlobManager.Infrastructure.Services.Common
{
    public class CommonService : ICommonService
    {
        private readonly IConfigHelper _configHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public readonly IDocumentDbContext _context;
        public CommonService(IConfigHelper configHelper, IWebHostEnvironment webHostEnvironment, IDocumentDbContext context)
        {
            _configHelper = configHelper;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        public async Task<ResponseViewModel> CreateDirectory(UploadViewModel model)
        {
            var mediaType = await _context.LookupMediaTypes!.FirstOrDefaultAsync(x => x.Id == (int)model.MediaType!);
            var result = new ResponseViewModel
            {
                AbsolutePath = mediaType!.MediaPath,
                RootThumbnailsUrl = mediaType.MediaPath + "/" + await _configHelper.GetConfig("AzureBlob.Media.Thumbnails.Root"),
                RootThumbnailsLocalUrl = _webHostEnvironment.WebRootPath + "/" + mediaType.MediaPath + "/" + await _configHelper.GetConfig("AzureBlob.Media.Thumbnails.Root")
            };
            if (!Directory.Exists(result.RootThumbnailsLocalUrl) && mediaType.AllowThumb == true)
            {
                Directory.CreateDirectory(result.RootThumbnailsLocalUrl);
            }
            return result;
        }

        public async Task<bool> DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<ImageFormat> GetEncoder(string extension)
        {
            ImageFormat encoder = null;

            extension = extension.Replace(".", "");

            var isSupported = Regex.IsMatch(extension, "gif|png|jpe?g", RegexOptions.IgnoreCase);

            if (isSupported)
            {
                switch (extension.ToLower())
                {
                    case "png":
                        encoder = ImageFormat.Png;
                        break;
                    case "jpg":
                        encoder = ImageFormat.Jpeg;
                        break;
                    case "jpeg":
                        encoder = ImageFormat.Jpeg;
                        break;
                    case "gif":
                        encoder = ImageFormat.Gif;
                        break;
                    default:
                        break;
                }
            }

            return await Task.FromResult(encoder);
        }

        public async Task<Bitmap> CreateThumbnail(Stream lcFilename, int lnWidth, int lnHeight)
        {
            System.Drawing.Bitmap bmpOut = null;
            try
            {
                Bitmap loBmp = new Bitmap(lcFilename);
                ImageFormat loFormat = loBmp.RawFormat;

                decimal lnRatio;
                int lnNewWidth = 0;
                int lnNewHeight = 0;

                //*** If the image is smaller than a thumbnail just return it
                if (loBmp.Width < lnWidth && loBmp.Height < lnHeight)
                    return loBmp;

                if (loBmp.Width > loBmp.Height)
                {
                    lnRatio = (decimal)lnWidth / loBmp.Width;
                    lnNewWidth = lnWidth;
                    decimal lnTemp = loBmp.Height * lnRatio;
                    lnNewHeight = (int)lnTemp;
                }
                else
                {
                    lnRatio = (decimal)lnHeight / loBmp.Height;
                    lnNewHeight = lnHeight;
                    decimal lnTemp = loBmp.Width * lnRatio;
                    lnNewWidth = (int)lnTemp;
                }
                bmpOut = new Bitmap(lnNewWidth, lnNewHeight);
                Graphics g = Graphics.FromImage(bmpOut);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.FillRectangle(Brushes.White, 0, 0, lnNewWidth, lnNewHeight);
                g.DrawImage(loBmp, 0, 0, lnNewWidth, lnNewHeight);

                loBmp.Dispose();
            }
            catch
            {
                return await Task.FromResult(bmpOut);
            }

            return await Task.FromResult(bmpOut);
        }
    }
}
