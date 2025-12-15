using My_Uni_Hub.Models.Pages;

namespace My_Uni_Hub.Services.Interfaces
{
    public interface IMaterialService
    {
        Task<List<Material>> GetAllAsync(CancellationToken ct = default);
        Task<List<Material>> GetByCourseAsync(int courseId, CancellationToken ct = default);
        Task<Material?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Material> CreateAsync(Material material, CancellationToken ct = default);
        Task UpdateAsync(Material material, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
