using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using My_Uni_Hub.Models.Pages;

namespace My_Uni_Hub.Services.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetAllAsync(CancellationToken ct);
        Task<Course?> GetByIdAsync(int id, CancellationToken ct);
        Task<Course?> GetByIdWithMaterialsAsync(int id, CancellationToken ct);
        Task CreateAsync(Course model, CancellationToken ct);
        Task UpdateAsync(Course model, CancellationToken ct);
        Task DeleteAsync(int id, CancellationToken ct);

        Task AddMaterialAsync(Material model, CancellationToken ct);
        Task<int> CountAsync();

        // Methods to provide dynamic select lists (faculties, departments)
        Task<IEnumerable<Faculty>> GetFacultiesAsync(CancellationToken ct);
        Task<IEnumerable<Department>> GetDepartmentsAsync(CancellationToken ct);
    }
}
