using Microsoft.EntityFrameworkCore;
using My_Uni_Hub.Models;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Services.Interfaces;

namespace My_Uni_Hub.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly MyUniDbContext _db;
        public StudentService(MyUniDbContext db) => _db = db;

        public async Task<List<Student>> GetAllAsync(CancellationToken ct = default)
            => await _db.Students.AsNoTracking()
                .Include(s => s.Faculty)
                .Include(s => s.Department)
                .OrderBy(s => s.FullName).ToListAsync(ct);

        // using Microsoft.EntityFrameworkCore; using Microsoft.Extensions.Logging;

        public async Task<Student> CreateAsync(Student student, CancellationToken ct = default)
        {
            student.CreatedAt = DateTime.UtcNow;
            _db.Students.Add(student);
            await _db.SaveChangesAsync(ct);
            return student; // بعد الحفظ Id متوفر
        }

        public async Task AssignAllCoursesToStudentAsync(int studentId, CancellationToken ct = default)
        {
            // Safety: atomic operation using transaction
            using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                // get all course ids
                var allCourseIds = await _db.Courses.Select(c => c.Id).ToListAsync(ct);
                if (!allCourseIds.Any())
                {
                    await tx.CommitAsync(ct);
                    return;
                }

                // existing for this student
                var existing = await _db.Set<CourseStudent>()
                                        .Where(cs => cs.StudentsId == studentId)
                                        .Select(cs => cs.CoursesId)
                                        .ToListAsync(ct);

                var toInsert = allCourseIds
                    .Where(id => !existing.Contains(id))
                    .Select(id => new CourseStudent { CoursesId = id, StudentsId = studentId })
                    .ToList();

                if (toInsert.Any())
                {
                    await _db.Set<CourseStudent>().AddRangeAsync(toInsert, ct);
                    await _db.SaveChangesAsync(ct);
                }

                await tx.CommitAsync(ct);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        public async Task<Student?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _db.Students
                .Include(s => s.Faculty)
                .Include(s => s.Department)
                .Include(s => s.CourseStudents)
                    .ThenInclude(cs => cs.Course)
                .FirstOrDefaultAsync(s => s.Id == id, ct);

        public async Task<Student?> GetByUserIdAsync(string userId, CancellationToken ct = default)
            => await _db.Students
                .Include(s => s.Faculty)
                .Include(s => s.Department)
                .Include(s => s.CourseStudents)
                    .ThenInclude(cs => cs.Course)
                .FirstOrDefaultAsync(s => s.UserId == userId, ct);




        public async Task UpdateAsync(Student student, CancellationToken ct = default)
        {
            _db.Students.Update(student);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            try
            {
                // حذف سجلات الربط (no memory load)
                _db.CourseStudents.RemoveRange(_db.CourseStudents.Where(x => x.StudentsId == id));

                // حذف التوابع
                _db.Answers.RemoveRange(_db.Answers.Where(a => a.StudentId == id));
                _db.Questions.RemoveRange(_db.Questions.Where(q => q.StudentId == id));
                _db.UniversityAgendas.RemoveRange(_db.UniversityAgendas.Where(u => u.StudentId == id));
                _db.LostItems.RemoveRange(_db.LostItems.Where(l => l.StudentId == id));
                _db.Opportunities.RemoveRange(_db.Opportunities.Where(o => o.StudentId == id));
                _db.DashboardNotifications.RemoveRange(_db.DashboardNotifications.Where(n => n.StudentId == id));

                // الآن احذف الطالب نفسه
                var student = await _db.Students.FindAsync(new object[] { id }, ct);
                if (student != null)
                {
                    _db.Students.Remove(student);
                }
                else
                {
                    // لو حابب: سجل لوج أو ارجع بدلالة أن الطالب غير موجود
                    await tx.CommitAsync(ct);
                    return;
                }

                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync(ct);
                // سجّل الخطأ هنا إذا عندك logging
                throw;
            }
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
            => await _db.Students.AnyAsync(s => s.Email == email, ct);
    }
}

