using Microsoft.AspNetCore.Http;

namespace EventBooking.API.Services
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName);
        void DeleteFile(string fileUrl);
    }
}