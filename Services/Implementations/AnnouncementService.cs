using Microsoft.EntityFrameworkCore;
using My_Uni_Hub.Models;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Services.Interfaces;

namespace My_Uni_Hub.Services.Implementations
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly MyUniDbContext _db;
        public AnnouncementService(MyUniDbContext db) => _db = db;

        public async Task<List<Announcement>> GetAllAsync(CancellationToken ct = default)
            => await _db.Announcements
                .AsNoTracking()
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync(ct);

        public async Task<Announcement?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _db.Announcements
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id, ct);

        public async Task<Announcement> CreateAsync(Announcement announcement, CancellationToken ct = default)
        {
            announcement.CreatedAt = DateTime.UtcNow;
            _db.Announcements.Add(announcement);
            await _db.SaveChangesAsync(ct);
            return announcement;
        }

        public async Task UpdateAsync(Announcement announcement, CancellationToken ct = default)
        {
            _db.Announcements.Update(announcement);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var e = await _db.Announcements.FindAsync(new object[] { id }, ct);
            if (e != null)
            {
                _db.Announcements.Remove(e);
                await _db.SaveChangesAsync(ct);
            }
        }

        public async Task<List<Announcement>> GetLatestAsync(int count = 1, CancellationToken ct = default)
        {
            return await _db.Announcements
                .AsNoTracking()
                .OrderByDescending(a => a.CreatedAt)
                .Take(count)
                .ToListAsync(ct);
        }

    }
}
