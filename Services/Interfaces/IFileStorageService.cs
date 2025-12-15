namespace My_Uni_Hub.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder, CancellationToken ct = default);
        Task DeleteFileAsync(string publicUrl, CancellationToken ct = default);
    }
}
