using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using My_Uni_Hub.Models;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Services.Interfaces;

namespace My_Uni_Hub.Controllers.Admin
{
    [Authorize(Roles = "Admin")]


    [Route("admin/opportunities")]
    public class OpportunityController : Controller
    {
        private readonly MyUniDbContext _db;
        private readonly IOpportunityService _service;

        public OpportunityController(MyUniDbContext db, IOpportunityService service)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        // GET: /admin/opportunities
        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var list = await _db.Opportunities
                .AsNoTracking()
                .Include(o => o.Faculty)
                .Include(o => o.Department)
                .OrderByDescending(o => o.PostedAt)
                .ToListAsync(ct);

            return View(list);
        }


        // GET: /admin/opportunities/create
        [HttpGet("create")]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await RepopulateSelects(new Opportunity(), ct);
            return View(new Opportunity());
        }

        // POST: /admin/opportunities/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Opportunity model, CancellationToken ct)
        {
            System.Diagnostics.Debug.WriteLine("DEBUG: Entered Create POST - " + DateTime.UtcNow);
            System.Diagnostics.Debug.WriteLine("DEBUG: ModelState.IsValid (before) = " + ModelState.IsValid);

            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var err in kvp.Value.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"VALIDATION: {kvp.Key} => {err.ErrorMessage}");
                    }
                }

                await RepopulateSelects(model, ct);
                return View(model);
            }

            if (model.FacultyId.HasValue && model.FacultyId.Value <= 0) model.FacultyId = null;
            if (model.DepartmentId.HasValue && model.DepartmentId.Value <= 0) model.DepartmentId = null;
            if (model.StudentId.HasValue && model.StudentId.Value <= 0) model.StudentId = null;

            if (model.DepartmentId.HasValue)
            {
                var deptExists = await _db.Departments.AsNoTracking().AnyAsync(d => d.Id == model.DepartmentId.Value, ct);
                if (!deptExists)
                {
                    ModelState.AddModelError(nameof(model.DepartmentId), "Selected department does not exist in this database.");
                    await RepopulateSelects(model, ct);
                    return View(model);
                }
            }

            model.Department = null;
            model.Faculty = null;
            model.Student = null;

            try
            {
                System.Diagnostics.Debug.WriteLine("DEBUG: About to call CreateAsync - " + DateTime.UtcNow);
                var created = await _service.CreateAsync(model, ct);
                System.Diagnostics.Debug.WriteLine("DEBUG: CreateAsync completed - Id=" + created.OpportunityId);

                return RedirectToAction("Index", new { saved = true });
            }
            catch (DbUpdateException dbex)
            {
                var msg = dbex.InnerException?.Message ?? dbex.Message;
                System.Diagnostics.Debug.WriteLine("DB ERROR in Create POST: " + msg);
                ModelState.AddModelError(string.Empty, "Database error: " + msg);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR in Create POST: " + ex.ToString());
                ModelState.AddModelError(string.Empty, "Unexpected error: " + ex.Message);
            }

            await RepopulateSelects(model, ct);
            return View(model);
        }




        // GET: /admin/opportunities/edit/{id}
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var item = await _db.Opportunities
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OpportunityId == id, ct);

            if (item == null) return NotFound();

            await RepopulateSelects(item, ct);
            return View(item);
        }

        // POST: /admin/opportunities/edit/{id}
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Opportunity model, CancellationToken ct)
        {
            if (id != model.OpportunityId) return BadRequest();

            if (!ModelState.IsValid)
            {
                await RepopulateSelects(model, ct);
                return View(model);
            }

            if (model.FacultyId.HasValue && model.FacultyId.Value <= 0) model.FacultyId = null;
            if (model.DepartmentId.HasValue && model.DepartmentId.Value <= 0) model.DepartmentId = null;

            if (model.DepartmentId.HasValue)
            {
                var dept = await _db.Departments.AsNoTracking().FirstOrDefaultAsync(d => d.Id == model.DepartmentId.Value, ct);
                if (dept == null)
                {
                    ModelState.AddModelError(nameof(model.DepartmentId), "Selected department does not exist.");
                    await RepopulateSelects(model, ct);
                    return View(model);
                }

                if (model.FacultyId.HasValue && dept.FacultyId.HasValue && dept.FacultyId.Value != model.FacultyId.Value)
                {
                    ModelState.AddModelError(nameof(model.DepartmentId), "Selected department does not belong to the chosen faculty.");
                    await RepopulateSelects(model, ct);
                    return View(model);
                }

                if (!model.FacultyId.HasValue && dept.FacultyId.HasValue)
                {
                    model.FacultyId = dept.FacultyId;
                }
            }

            model.Department = null;
            model.Faculty = null;
            model.Student = null;

            await _service.UpdateAsync(model, ct);
            return RedirectToAction("Index");
        }


        // POST: /admin/opportunities/delete/{id}

        [HttpPost("delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var exists = await _db.Opportunities.AsNoTracking().AnyAsync(o => o.OpportunityId == id, ct);
            if (!exists) return NotFound();

            await _service.DeleteAsync(id, ct);
            return RedirectToAction("Index");
        }



        // GET: /admin/opportunities/departments/{facultyId}
        [HttpGet("departments/{facultyId:int}")]
        public async Task<IActionResult> GetDepartments(int facultyId, CancellationToken ct)
        {
            if (facultyId <= 0) return Json(Array.Empty<object>());

            var list = await _db.Departments
                .AsNoTracking()
                .Where(d => d.FacultyId == facultyId)
                .OrderBy(d => d.Name)
                .Select(d => new { d.Id, d.Name })
                .ToListAsync(ct);

            return Json(list);
        }


        private async Task RepopulateSelects(Opportunity model, CancellationToken ct)
        {
            var faculties = await _db.Faculties.AsNoTracking().OrderBy(f => f.Name).ToListAsync(ct);
            ViewBag.Faculties = new SelectList(faculties, "Id", "Name", model?.FacultyId);

            List<Department> deptList;
            if (model?.FacultyId != null && model.FacultyId > 0)
            {
                deptList = await _db.Departments
                    .AsNoTracking()
                    .Where(d => d.FacultyId == model.FacultyId.Value)
                    .OrderBy(d => d.Name)
                    .ToListAsync(ct);
            }
            else
            {
                deptList = new List<Department>();
            }

            ViewBag.Departments = new SelectList(deptList, "Id", "Name", model?.DepartmentId);
        }
    }
}