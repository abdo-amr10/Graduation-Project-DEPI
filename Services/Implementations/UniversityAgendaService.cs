using Microsoft.EntityFrameworkCore;
using My_Uni_Hub.Models;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Services.Interfaces;

namespace My_Uni_Hub.Services.Implementations
{
    public class UniversityAgendaService : IUniversityAgendaService
    {
        private readonly MyUniDbContext _context;

        public UniversityAgendaService(MyUniDbContext context)
        {
            _context = context;
        }

        public async Task<List<UniversityAgenda>> GetAllEventsAsync()
        {
            return await _context.UniversityAgendas
                .Include(a => a.Faculty)
                .Include(a => a.Department)
                .Include(a => a.Student)
                .OrderBy(a => a.StartDate)
                .ToListAsync();
        }

        public async Task<UniversityAgenda> GetEventByIdAsync(int id)
        {
            return await _context.UniversityAgendas
                .Include(a => a.Faculty)
                .Include(a => a.Department)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<UniversityAgenda>> GetEventsByTypeAsync(string type)
        {
            return await _context.UniversityAgendas
                .Include(a => a.Faculty)
                .Include(a => a.Department)
                .Include(a => a.Student)
                .Where(a => a.Type == type)
                .OrderBy(a => a.StartDate)
                .ToListAsync();
        }

        public async Task<List<UniversityAgenda>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.UniversityAgendas
                .Include(a => a.Faculty)
                .Include(a => a.Department)
                .Include(a => a.Student)
                .Where(a => a.StartDate == startDate && a.EndDate == endDate)
                .OrderBy(a => a.StartDate)
                .ToListAsync();
        }

        public async Task<List<UniversityAgenda>> GetUpcomingEventsAsync(int days = 7)
        {
            var today = DateTime.Now.Date;
            var futureDate = today.AddDays(days);

            return await _context.UniversityAgendas
                .Include(a => a.Faculty)
                .Include(a => a.Department)
                .Include(a => a.Student)
                .Where(a => a.StartDate >= today && a.StartDate <= futureDate)
                .OrderBy(a => a.StartDate)
                .ToListAsync();
        }

        public async Task<List<UniversityAgenda>> GetEventsByFacultyAsync(int facultyId)
        {
            return await _context.UniversityAgendas
                .Include(a => a.Faculty)
                .Include(a => a.Department)
                .Include(a => a.Student)
                .Where(a => a.FacultyId == facultyId)
                .OrderBy(a => a.StartDate)
                .ToListAsync();
        }

        public async Task<List<UniversityAgenda>> GetEventsByDepartmentAsync(int departmentId)
        {
            return await _context.UniversityAgendas
                .Include(a => a.Faculty)
                .Include(a => a.Department)
                .Include(a => a.Student)
                .Where(a => a.DepartmentId == departmentId)
                .OrderBy(a => a.StartDate)
                .ToListAsync();
        }

        public async Task<List<UniversityAgenda>> GetEventsByStudentAsync(int studentId)
        {
            return await _context.UniversityAgendas
                .Include(a => a.Faculty)
                .Include(a => a.Department)
                .Include(a => a.Student)
                .Where(a => a.StudentId == studentId)
                .OrderBy(a => a.StartDate)
                .ToListAsync();
        }



        public async Task<UniversityAgenda> CreateEventAsync(UniversityAgenda agenda)
        {
            _context.UniversityAgendas.Add(agenda);
            await _context.SaveChangesAsync();
            return agenda;
        }

        public async Task<bool> UpdateEventAsync(UniversityAgenda agenda)
        {
            try
            {
                _context.Entry(agenda).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventExistsAsync(agenda.Id))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var agenda = await _context.UniversityAgendas.FindAsync(id);
            if (agenda == null)
            {
                return false;
            }

            _context.UniversityAgendas.Remove(agenda);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EventExistsAsync(int id)
        {
            return await _context.UniversityAgendas.AnyAsync(e => e.Id == id);
        }
    }
}

