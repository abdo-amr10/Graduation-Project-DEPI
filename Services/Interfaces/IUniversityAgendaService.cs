using My_Uni_Hub.Models.Pages;

namespace My_Uni_Hub.Services.Interfaces
{
    public interface IUniversityAgendaService
    {
        Task<List<UniversityAgenda>> GetAllEventsAsync();
        Task<UniversityAgenda> GetEventByIdAsync(int id);
        Task<List<UniversityAgenda>> GetEventsByTypeAsync(string type);
        Task<List<UniversityAgenda>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<UniversityAgenda>> GetUpcomingEventsAsync(int days = 7);
        Task<List<UniversityAgenda>> GetEventsByFacultyAsync(int facultyId);
        Task<List<UniversityAgenda>> GetEventsByDepartmentAsync(int departmentId);
        Task<List<UniversityAgenda>> GetEventsByStudentAsync(int studentId);
        Task<UniversityAgenda> CreateEventAsync(UniversityAgenda agenda);
        Task<bool> UpdateEventAsync(UniversityAgenda agenda);
        Task<bool> DeleteEventAsync(int id);
        Task<bool> EventExistsAsync(int id);
    }
}
