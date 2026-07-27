
using Bookify.Web.Core.Consts;
using Bookify.Web.Core.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Bookify.Web.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _webHost;
        private List<string> _allowedExtentions = new List<string>() { ".jpg", ".png", ".jepg" };
        private int _maxAllowSize = 2 * 1024 * 1024;
        public ImageService(IWebHostEnvironment webHost)
        {
            _webHost = webHost;
        }



        public async Task<(bool IsUploaded, string? ErrorMessage)> UploadAsync(IFormFile image, string imageName, string folderPath, bool hasThumbnail)
        {
            var extension = Path.GetExtension(image.FileName);
            if (!_allowedExtentions.Contains(extension))
            {
                return (IsUploaded: false, ErrorMessage: Errors.NotAllowedExtention);
            }
            if (image.Length > _maxAllowSize)
            {
                return (IsUploaded: false, ErrorMessage: Errors.MaxSize);
            }



            var path = Path.Combine($"{_webHost.WebRootPath}/{folderPath}", imageName);
            using var stream = System.IO.File.Create(path);
            await image.CopyToAsync(stream);

            stream.Dispose();

            
            if (hasThumbnail)
            {
                using var Loadedimage = SixLabors.ImageSharp.Image.Load(image.OpenReadStream());
                var ratio = (float)Loadedimage.Width / 200;
                var height = Loadedimage.Height / ratio;
                Loadedimage.Mutate(i => i.Resize(width: 200, height: (int)height));
                var Thumpath = Path.Combine($"{_webHost.WebRootPath}/{folderPath}/Thumb", imageName);
                Loadedimage.Save(Thumpath);

            }
            return (IsUploaded: true, ErrorMessage: null);

        }

        public void Delete(string imagePath, string? thumnailPath=null)
        {
            var oldPathImage = $"{_webHost.WebRootPath}{imagePath}";

            //Delete originImage
            if (System.IO.File.Exists(oldPathImage))
            {
                System.IO.File.Delete(oldPathImage);
            }

            //Delete ThumnailImage
            if (!string.IsNullOrEmpty(thumnailPath))
            {
                var oldPathThum = $"{_webHost.WebRootPath}{thumnailPath}";
                if (System.IO.File.Exists(oldPathThum))
                {
                    System.IO.File.Delete(oldPathThum);
                }
            }
        }
    }
}
