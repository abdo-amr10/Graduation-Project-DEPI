using My_Uni_Hub.Models.Pages;

namespace My_Uni_Hub.Services.Interfaces
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllAsync(CancellationToken ct = default);
        Task<Student?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Student?> GetByUserIdAsync(string userId, CancellationToken ct = default);
        Task<Student> CreateAsync(Student student, CancellationToken ct = default);
        Task AssignAllCoursesToStudentAsync(int studentId, CancellationToken ct);

        Task UpdateAsync(Student student, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    }
}
