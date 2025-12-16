using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Models.ViewModels;
using My_Uni_Hub.Services.Interfaces;
using System.Threading.Tasks;

namespace My_Uni_Hub.Controllers.Admin
{
    [Authorize(Roles = "Admin")]

    [Route("admin/courses")]
    public class CoursesController : Controller
    {
        private readonly ICourseService _courses;

        public CoursesController(ICourseService courses)
        {
            _courses = courses;
        }

        // GET: /admin/courses
        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var list = await _courses.GetAllAsync(ct);
            return View(list);
        }

        // GET: /admin/courses/create
        [HttpGet("create")]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var faculties = await _courses.GetFacultiesAsync(ct);
            var departments = await _courses.GetDepartmentsAsync(ct);

            ViewBag.Faculties = new SelectList(faculties, "Id", "Name");
            ViewBag.Departments = new SelectList(departments, "Id", "Name");

            return View(new Course());
        }

        // POST: /admin/courses/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                var faculties = await _courses.GetFacultiesAsync(ct);
                var departments = await _courses.GetDepartmentsAsync(ct);

                ViewBag.Faculties = new SelectList(faculties, "Id", "Name", model.FacultyId);
                ViewBag.Departments = new SelectList(departments, "Id", "Name", model.DepartmentId);

                return View(model);
            }

            await _courses.CreateAsync(model, ct);

            return RedirectToAction(nameof(Index));
        }

        // GET: /admin/courses/edit/{id}
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var course = await _courses.GetByIdAsync(id, ct);
            if (course == null) return NotFound();

            var vm = new EditCourseVM
            {
                Id = course.Id,
                Name = course.Name,
                CourseCode = course.CourseCode,
                Description = course.Description,
                FacultyId = course.FacultyId,
                DepartmentId = course.DepartmentId,
                Semester = course.Semester
            };

            ViewBag.Faculties = new SelectList(await _courses.GetFacultiesAsync(ct), "Id", "Name", vm.FacultyId);
            ViewBag.Departments = new SelectList(await _courses.GetDepartmentsAsync(ct), "Id", "Name", vm.DepartmentId);

            return View(vm);
        }


        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditCourseVM model, CancellationToken ct)
        {
            if (model.Id == 0)
                model.Id = id;

            if (!ModelState.IsValid)
            {
                ViewBag.Faculties = new SelectList(await _courses.GetFacultiesAsync(ct), "Id", "Name", model.FacultyId);
                ViewBag.Departments = new SelectList(await _courses.GetDepartmentsAsync(ct), "Id", "Name", model.DepartmentId);

                return View(model);
            }

            var existing = await _courses.GetByIdAsync(model.Id, ct);
            if (existing == null)
                return NotFound();
            existing.Name = model.Name;
            existing.CourseCode = model.CourseCode;
            existing.Description = model.Description;
            existing.Semester = model.Semester;
            existing.FacultyId = model.FacultyId;
            existing.DepartmentId = model.DepartmentId;

            await _courses.UpdateAsync(existing, ct);

            return RedirectToAction(nameof(Index));
        }


        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _courses.DeleteAsync(id, ct);
            return RedirectToAction(nameof(Index));
        }

    }
}
