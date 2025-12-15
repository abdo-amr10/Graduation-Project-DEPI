using Microsoft.EntityFrameworkCore;
using My_Uni_Hub.Models;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Services.Interfaces;

namespace My_Uni_Hub.Services.Implementations
{
    public class LostItemService : ILostItemService
    {
        private readonly MyUniDbContext _db;
        public LostItemService(MyUniDbContext db) => _db = db;

        public async Task<List<LostItem>> GetAllAsync(CancellationToken ct = default)
            => await _db.Set<LostItem>()
                        .AsNoTracking()
                        .OrderByDescending(x => x.PostedAt)
                        .ToListAsync(ct);

        public async Task<LostItem?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _db.Set<LostItem>().FindAsync(new object[] { id }, ct);

        public async Task<LostItem> CreateAsync(LostItem item, CancellationToken ct = default)
        {
            if (item.PostedAt == default) item.PostedAt = DateTime.UtcNow;
            _db.LostItems.Add(item);
            await _db.SaveChangesAsync(ct);
            return item;
        }

        public async Task UpdateAsync(LostItem item, CancellationToken ct = default)
        {
            var tracked = await _db.LostItems.FindAsync(new object[] { item.Id }, ct);
            if (tracked == null) return;

            _db.Entry(tracked).CurrentValues.SetValues(item);
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
    }
}
