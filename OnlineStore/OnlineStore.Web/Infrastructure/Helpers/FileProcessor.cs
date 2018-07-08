namespace OnlineStore.Web.Infrastructure.Helpers
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class FileProcessor : IFileProcessor
    {
        private IHostingEnvironment hostingEnvironment;

        public FileProcessor(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
        
        public string EnsureDirectoryExist(int productId)
        {
            var productImagesDirectory = $"{WebConstants.ProductPicturesServerPath}{WebConstants.ProductPicturesDirectoryNamePrefix}{productId}";
            var uploadDirectory = Path
                .Combine(hostingEnvironment.WebRootPath, productImagesDirectory);

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            return uploadDirectory;
        }

        public async Task SaveToFullPath(string fullPath, IFormFile file)
        {
            // Add the original picture to filesystem
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }

        public string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            var uniqueFileName = string.Format("{0}_{1}{2}",
                        Path.GetFileNameWithoutExtension(fileName),
                        Guid.NewGuid().ToString().Substring(0, 6),
                        Path.GetExtension(fileName));

            return uniqueFileName;
        }

        public string GetFullPath(string directory, string fileName)
            => Path.Combine(directory, fileName);

        public bool DeleteFile(string shortPath)
        {
            var fullPath = Path.Combine(this.hostingEnvironment.WebRootPath, shortPath.TrimStart('\\'));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }

            return false;
        }

        public bool DeleteDirectory(string shortPath)
        {
            var fullPath = Path.Combine(this.hostingEnvironment.WebRootPath, shortPath.TrimStart('\\'));

            if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
                return true;
            }

            return false;
        }
    }
}
