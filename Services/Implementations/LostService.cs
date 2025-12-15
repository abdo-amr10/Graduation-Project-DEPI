using Microsoft.EntityFrameworkCore;
using My_Uni_Hub.Models;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Services.Interfaces;

namespace My_Uni_Hub.Services.Implementations
{
    public class LostService : ILostService
    {
        private readonly MyUniDbContext _db;
        public LostService(MyUniDbContext db) => _db = db;

        public async Task<List<LostItem>> GetAllAsync(CancellationToken ct = default)
            => await _db.LostItems.AsNoTracking().OrderByDescending(l => l.PostedAt).ToListAsync(ct);

        public async Task<LostItem?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _db.LostItems.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id, ct);

        public async Task<LostItem> CreateAsync(LostItem item, CancellationToken ct = default)
        {
            item.PostedAt = DateTime.UtcNow;
            _db.LostItems.Add(item);
            await _db.SaveChangesAsync(ct);
            return item;
        }

        public async Task UpdateAsync(LostItem item, CancellationToken ct = default)
        {
            _db.LostItems.Update(item);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var e = await _db.LostItems.FindAsync(new object[] { id }, ct);
            if (e != null)
            {
                _db.LostItems.Remove(e);
                await _db.SaveChangesAsync(ct);
            }
        }

        public async Task MarkFoundAsync(int id, CancellationToken ct = default)
        {
            var e = await _db.LostItems.FindAsync(new object[] { id }, ct);
            if (e != null)
            {
                e.IsFound = true;
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
