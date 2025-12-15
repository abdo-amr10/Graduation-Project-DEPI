using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using My_Uni_Hub.Models;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Models.ViewModels;
using My_Uni_Hub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace My_Uni_Hub.Controllers.Admin
{
    [Route("admin/student")]
    public class StudentController : Controller
    {
        private readonly IStudentService _students;
        private readonly IFileStorageService _files;
        private readonly MyUniDbContext _db; // required to build select lists

        public StudentController(IStudentService students, IFileStorageService files, MyUniDbContext db)
        {
            _students = students;
            _files = files;
            _db = db;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var list = await _students.GetAllAsync(ct);
            return View(list);
        }

        [HttpGet("details/{id:int}")]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var s = await _students.GetByIdAsync(id, ct);
            if (s == null) return NotFound();
            return View(s);
        }

        // helper to populate ViewBag for selects
        private async Task PopulateFacultyDepartmentSelectsAsync(int? selectedFacultyId = null, int? selectedDepartmentId = null, CancellationToken ct = default)
        {
            var faculties = await _db.Faculties.AsNoTracking().OrderBy(f => f.Name).ToListAsync(ct);
            var departments = await _db.Departments.AsNoTracking().OrderBy(d => d.Name).ToListAsync(ct);

            ViewBag.Faculties = new SelectList(faculties, "Id", "Name", selectedFacultyId);
            ViewBag.Departments = new SelectList(departments, "Id", "Name", selectedDepartmentId);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateFacultyDepartmentSelectsAsync(null, null, ct);
            return View(new StudentCreateVM());
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentCreateVM vm, IFormFile? photo, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateFacultyDepartmentSelectsAsync(vm.FacultyId, vm.DepartmentId, ct);
                return View(vm);
            }

            if (await _students.EmailExistsAsync(vm.Email, ct))
            {
                ModelState.AddModelError(nameof(vm.Email), "Email already exists.");
                await PopulateFacultyDepartmentSelectsAsync(vm.FacultyId, vm.DepartmentId, ct);
                return View(vm);
            }

            var student = new Student
            {
                FullName = vm.FullName,
                Email = vm.Email,
                AcademicYear = vm.AcademicYear,
                DepartmentId = vm.DepartmentId,
                FacultyId = vm.FacultyId,
                Bio = vm.Bio,
                IsVerified = vm.IsVerified,
                CreatedAt = DateTime.UtcNow
            };

            if (photo != null)
                student.PhotoUrl = await _files.SaveFileAsync(photo, "profiles", ct);

            await _students.CreateAsync(student, ct);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var s = await _students.GetByIdAsync(id, ct);
            if (s == null) return NotFound();

            var vm = new StudentEditVM
            {
                Id = s.Id,
                FullName = s.FullName,
                Email = s.Email,
                AcademicYear = s.AcademicYear,
                DepartmentId = s.DepartmentId,
                FacultyId = s.FacultyId,
                Bio = s.Bio,
                PhotoUrl = s.PhotoUrl
            };

            await PopulateFacultyDepartmentSelectsAsync(vm.FacultyId, vm.DepartmentId, ct);
            return View(vm);
        }

        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentEditVM vm, IFormFile? photo, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateFacultyDepartmentSelectsAsync(vm.FacultyId, vm.DepartmentId, ct);
                return View(vm);
            }

            var s = await _students.GetByIdAsync(vm.Id, ct);
            if (s == null) return NotFound();

            if (!string.Equals(s.Email, vm.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await _students.EmailExistsAsync(vm.Email, ct))
                {
                    ModelState.AddModelError(nameof(vm.Email), "Email already exists.");
                    await PopulateFacultyDepartmentSelectsAsync(vm.FacultyId, vm.DepartmentId, ct);
                    return View(vm);
                }
            }

            s.FullName = vm.FullName;
            s.Email = vm.Email;
            s.AcademicYear = vm.AcademicYear;
            s.DepartmentId = vm.DepartmentId;
            s.FacultyId = vm.FacultyId;
            s.Bio = vm.Bio;

            if (photo != null)
            {
                if (!string.IsNullOrWhiteSpace(s.PhotoUrl))
                    await _files.DeleteFileAsync(s.PhotoUrl, ct);

                s.PhotoUrl = await _files.SaveFileAsync(photo, "profiles", ct);
            }

            await _students.UpdateAsync(s, ct);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var s = await _students.GetByIdAsync(id, ct);
            if (s == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(s.PhotoUrl))
                await _files.DeleteFileAsync(s.PhotoUrl, ct);

            await _students.DeleteAsync(id, ct);
            return RedirectToAction(nameof(Index));
        }
    }
}
