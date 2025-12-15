namespace My_Uni_Hub.Services.Interfaces
{
    public interface ILocalFileStorageService
    {
        Task<string> UploadAsync(IFormFile file);
        void Delete(string filePath);
    }
}
