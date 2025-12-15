using My_Uni_Hub.Models.Pages;

namespace My_Uni_Hub.Services.Interfaces
{
    public interface IAnnouncementService
    {
        Task<List<Announcement>> GetAllAsync(CancellationToken ct = default);
        Task<Announcement?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Announcement> CreateAsync(Announcement announcement, CancellationToken ct = default);
        Task UpdateAsync(Announcement announcement, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<List<Announcement>> GetLatestAsync(int count = 1, CancellationToken ct = default);

    }
}
