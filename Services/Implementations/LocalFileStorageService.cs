using My_Uni_Hub.Services.Interfaces;

namespace My_Uni_Hub.Services.Implementations
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        public LocalFileStorageService(IWebHostEnvironment env) => _env = env;

        public async Task<string> SaveFileAsync(IFormFile file, string folder, CancellationToken ct = default)
        {
            var uploads = Path.Combine(_env.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploads);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploads, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream, ct);

            return $"/uploads/{folder}/{fileName}";
        }

        public Task DeleteFileAsync(string publicUrl, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(publicUrl)) return Task.CompletedTask;

            var rel = publicUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var full = Path.Combine(_env.WebRootPath, rel);

            if (File.Exists(full)) File.Delete(full);

            return Task.CompletedTask;
        }
    }
}
