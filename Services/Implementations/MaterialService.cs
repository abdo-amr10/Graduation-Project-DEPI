using Microsoft.EntityFrameworkCore;
using My_Uni_Hub.Models;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Services.Interfaces;

namespace My_Uni_Hub.Services.Implementations
{
    public class MaterialService : IMaterialService
    {
        private readonly MyUniDbContext _db;

        public MaterialService(MyUniDbContext db) => _db = db;

        public async Task<List<Material>> GetAllAsync(CancellationToken ct = default)
            => await _db.Materials
                        .AsNoTracking()
                        .Include(m => m.Course)
                        .OrderByDescending(m => m.UploadDate)
                        .ToListAsync(ct);

        public async Task<List<Material>> GetByCourseAsync(int courseId, CancellationToken ct = default)
            => await _db.Materials
                        .AsNoTracking()
                        .Where(m => m.CourseId == courseId)
                        .Include(m => m.Course)
                        .OrderByDescending(m => m.UploadDate)
                        .ToListAsync(ct);

        public async Task<Material?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _db.Materials
                        .AsNoTracking()
                        .Include(m => m.Course)
                        .FirstOrDefaultAsync(m => m.Id == id, ct);

        public async Task<Material> CreateAsync(Material material, CancellationToken ct = default)
        {
            material.UploadDate = DateTime.UtcNow;
            _db.Materials.Add(material);
            await _db.SaveChangesAsync(ct);
            return material;
        }

        public async Task UpdateAsync(Material material, CancellationToken ct = default)
        {
            _db.Materials.Update(material);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var e = await _db.Materials.FindAsync(new object[] { id }, ct);
            if (e != null)
            {
                _db.Materials.Remove(e);
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
