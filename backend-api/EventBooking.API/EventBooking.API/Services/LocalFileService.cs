using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace EventBooking.API.Services
{
    public class LocalFileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public LocalFileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ hoặc rỗng.");

            // 1. Chỉ định thư mục lưu trữ
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads", folderName);
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // 2. Tạo tên file duy nhất (Bảo mật & chống ghi đè)
            var fileExtension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}_{DateTime.UtcNow.Ticks}{fileExtension}";
            var physicalPath = Path.Combine(uploadPath, uniqueFileName);

            // 3. Copy file vào ổ cứng
            using (var fileStream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // 4. Trả về đường dẫn tương đối để lưu vào Database
            return $"/uploads/{folderName}/{uniqueFileName}";
        }

        public void DeleteFile(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            // Xóa file vật lý khi User muốn xóa ảnh khỏi Portfolio
            var physicalPath = Path.Combine(_env.WebRootPath, fileUrl.TrimStart('/'));
            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }
    }
}