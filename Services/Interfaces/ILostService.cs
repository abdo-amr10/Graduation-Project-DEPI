using My_Uni_Hub.Models.Pages;

namespace My_Uni_Hub.Services.Interfaces
{
    public interface ILostService
    {
        Task<List<LostItem>> GetAllAsync(CancellationToken ct = default);
        Task<LostItem?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<LostItem> CreateAsync(LostItem item, CancellationToken ct = default);
        Task UpdateAsync(LostItem item, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task MarkFoundAsync(int id, CancellationToken ct = default);
    }
}
