using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using My_Uni_Hub.Models;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Models.ViewModels;
using My_Uni_Hub.Services.Interfaces;
using System.IO;

namespace My_Uni_Hub.Controllers.Admin
{
    [Route("admin/materials")]
    public class MaterialController : Controller
    {
        private readonly IMaterialService _materials;
        private readonly IFileStorageService _files;
        private readonly MyUniDbContext _db;

        // 10 MB
        private readonly long _maxUploadBytes = 10 * 1024 * 1024;

        public MaterialController(IMaterialService materials, IFileStorageService files, MyUniDbContext db)
        {
            _materials = materials;
            _files = files;
            _db = db;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var list = await _materials.GetAllAsync(ct);
            return View(list);
        }

        private async Task PopulateCoursesAsync(int? selectedCourseId = null, CancellationToken ct = default)
        {
            var courses = await _db.Courses.AsNoTracking().OrderBy(c => c.Name).ToListAsync(ct);
            ViewBag.Courses = new SelectList(courses, "Id", "Name", selectedCourseId);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateCoursesAsync(null, ct);
            return View(new Material()); 
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Material model, IFormFile? file, CancellationToken ct)
        {
            await PopulateCoursesAsync(model.CourseId, ct);

            if (!ModelState.IsValid) return View(model);

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Please attach a file.");
                return View(model);
            }

            if (file.Length > _maxUploadBytes)
            {
                ModelState.AddModelError("file", "File is too large (max 10MB).");
                return View(model);
            }

            var allowed = new[] { ".pdf", ".docx", ".pptx", ".zip", ".png", ".jpg", ".jpeg", ".mp4" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext))
            {
                ModelState.AddModelError("file", "File type not allowed.");
                return View(model);
            }

            // Save file
            model.FilePath = await _files.SaveFileAsync(file, "materials", ct);
            model.FileType = ext;
            model.UploadDate = DateTime.UtcNow;

            await _materials.CreateAsync(model, ct);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var m = await _materials.GetByIdAsync(id, ct);
            if (m == null) return NotFound();

            await PopulateCoursesAsync(m.CourseId, ct);

            var vm = new MaterialEditVM
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                Category = m.Category,
                CourseId = m.CourseId,
                ExistingFilePath = m.FilePath,
                ExistingFileName = string.IsNullOrWhiteSpace(m.FilePath) ? null : Path.GetFileName(m.FilePath)
            };

            return View(vm);
        }

        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MaterialEditVM vm, IFormFile? file, CancellationToken ct)
        {
            await PopulateCoursesAsync(vm.CourseId, ct);

            if (!ModelState.IsValid) return View(vm);

            var m = await _materials.GetByIdAsync(vm.Id, ct);
            if (m == null) return NotFound();

            m.Title = vm.Title;
            m.Description = vm.Description;
            m.Category = vm.Category;
            m.CourseId = vm.CourseId;

            if (file != null)
            {
                if (file.Length == 0 || file.Length > _maxUploadBytes)
                {
                    ModelState.AddModelError("file", "File is empty or too large (max 10MB).");
                    return View(vm);
                }

                var allowed = new[] { ".pdf", ".docx", ".pptx", ".zip", ".png", ".jpg", ".jpeg", ".mp4" };
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("file", "File type not allowed.");
                    return View(vm);
                }

                if (!string.IsNullOrWhiteSpace(m.FilePath))
                    await _files.DeleteFileAsync(m.FilePath, ct);

                m.FilePath = await _files.SaveFileAsync(file, "materials", ct);
                m.FileType = ext;
                m.UploadDate = DateTime.UtcNow;
            }

            await _materials.UpdateAsync(m, ct);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var m = await _materials.GetByIdAsync(id, ct);
            if (m == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(m.FilePath))
                await _files.DeleteFileAsync(m.FilePath, ct);

            await _materials.DeleteAsync(id, ct);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("details/{id:int}")]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var m = await _materials.GetByIdAsync(id, ct);
            if (m == null) return NotFound();
            return View(m);
        }
    }
}
