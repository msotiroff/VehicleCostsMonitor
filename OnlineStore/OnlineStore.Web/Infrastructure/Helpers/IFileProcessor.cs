namespace OnlineStore.Web.Infrastructure.Helpers
{
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;

    public interface IFileProcessor
    {
        string EnsureDirectoryExist(int productId);

        string GetUniqueFileName(string name);

        Task SaveToFullPath(string fullPath, IFormFile file);

        string GetFullPath(string directory, string fileName);

        bool DeleteFile(string shortPath);

        bool DeleteDirectory(string shortPath);
    }
}
