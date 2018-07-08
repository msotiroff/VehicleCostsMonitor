namespace OnlineStore.Web.Infrastructure.Helpers
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.Linq;
    using Microsoft.AspNetCore.Http;

    public class ImageProcessor : IImageProcessor
    {
        public byte[] GetBytesFromImage(IFormFile file)
        {
            var fileExtension = "." + file.FileName.Split(".").LastOrDefault();

            if (this.ValidateImageExtenssion(fileExtension))
            {
                if (file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        var fileBytes = memoryStream.ToArray();
                        return fileBytes;
                    }
                }
            }

            return null;
        }

        public void ProcessImage(string path)
        {
            var imgOriginal = Image.FromFile(path);
            
            var resizedImg =
                    this.ResizePicture(imgOriginal, WebConstants.PictureMaxWidth, WebConstants.PictureMaxHeight);

            imgOriginal.Dispose();

            if (resizedImg != null)
            {
                resizedImg.Save(path);
            }
        }

        public bool ValidateImageExtenssion(string extension)
        {
            switch (extension.ToLower())
            {
                case ".jpg":
                    return true;
                case ".png":
                    return true;
                case ".jpeg":
                    return true;
                case ".bmp":
                    return true;
                default:
                    return false;
            }
        }

        private Image ResizePicture(Image imgOriginal, int pictureMaxWidth, int pictureMaxHeight)
        {
            // Height will increase the width proportionally:
            int height = Math.Min(pictureMaxHeight, imgOriginal.Height);
            int width = (height * imgOriginal.Width) / imgOriginal.Height;

            var resizedImg = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(resizedImg))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(imgOriginal, 0, 0, width, height);
            }

            return resizedImg;
        }
    }
}
