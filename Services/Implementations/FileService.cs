using Microsoft.EntityFrameworkCore;
using My_Uni_Hub.Models;
using My_Uni_Hub.Services.Interfaces;

namespace My_Uni_Hub.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly MyUniDbContext _db;

        public FileService(MyUniDbContext db)
        {
            _db = db;
        }

        public Task<int> CountAsync()
        {
            return _db.Materials.CountAsync();
        }
    }
}
