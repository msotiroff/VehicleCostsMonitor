namespace OnlineStore.Web.Infrastructure.Helpers
{
    using Microsoft.AspNetCore.Http;

    public interface IImageProcessor
    {
        bool ValidateImageExtenssion(string filename);

        void ProcessImage(string path);

        byte[] GetBytesFromImage(IFormFile file);
    }
}
