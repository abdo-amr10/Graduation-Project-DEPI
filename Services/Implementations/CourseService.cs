using Microsoft.EntityFrameworkCore;
using My_Uni_Hub.Models;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace My_Uni_Hub.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly MyUniDbContext _db;

        public CourseService(MyUniDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Course>> GetAllAsync(CancellationToken ct)
        {
            return await _db.Courses
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync(ct);
        }

        public async Task<Course?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _db.Courses
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<Course?> GetByIdWithMaterialsAsync(int id, CancellationToken ct)
        {
            return await _db.Courses
                .Include(c => c.Materials)
                .Include(c => c.Faculty)
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task CreateAsync(Course model, CancellationToken ct)
        {
            _db.Courses.Add(model);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Course model, CancellationToken ct)
        {
            var tracked = await _db.Courses.FindAsync(new object[] { model.Id }, ct);
            if (tracked == null) return;

            _db.Entry(tracked).CurrentValues.SetValues(model);

            await _db.SaveChangesAsync(ct);
        }


        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            var c = await _db.Courses.FindAsync(new object[] { id }, ct);
            if (c == null) return;
            _db.Courses.Remove(c);
            await _db.SaveChangesAsync(ct);
        }

        public async Task AddMaterialAsync(Material model, CancellationToken ct)
        {
            _db.Materials.Add(model);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<Faculty>> GetFacultiesAsync(CancellationToken ct)
        {
            return await _db.Faculties
                .AsNoTracking()
                .OrderBy(f => f.Name)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Department>> GetDepartmentsAsync(CancellationToken ct)
        {
            return await _db.Departments
                .AsNoTracking()
                .OrderBy(d => d.Name)
                .ToListAsync(ct);
        }

        public Task<int> CountAsync()
        {
            return _db.Courses.CountAsync();
        }
    }
}
