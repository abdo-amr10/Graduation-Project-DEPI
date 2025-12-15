using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using My_Uni_Hub.Models;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace My_Uni_Hub.Controllers
{
    [Route("admin/announcements")]
    public class AnnouncementsController : Controller
    {
        private readonly IAnnouncementService _ann;
        private readonly MyUniDbContext _db;

        public AnnouncementsController(IAnnouncementService ann, MyUniDbContext db)
        {
            _ann = ann ?? throw new ArgumentNullException(nameof(ann));
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        private async Task PopulateFacultiesAsync(int? selectedFacultyId = null, CancellationToken ct = default)
        {
            try
            {
                var faculties = await _db.Faculties
                                         .AsNoTracking()
                                         .OrderBy(f => f.Name)
                                         .ToListAsync(ct);

                var items = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = "",
                        Text = "All Faculties",
                        Selected = !selectedFacultyId.HasValue
                    }
                };

                items.AddRange(faculties.Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.Name,
                    Selected = selectedFacultyId.HasValue && selectedFacultyId.Value == f.Id
                }));

                ViewBag.Faculties = items;
            }
            catch (OperationCanceledException)
            {

                ViewBag.Faculties = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = "",
                        Text = "All Faculties",
                        Selected = !selectedFacultyId.HasValue
                    }
                };
            }
            catch (Exception ex)
            {

                ViewBag.Faculties = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "All Faculties", Selected = !selectedFacultyId.HasValue }
                };
            }
        }

        // GET: /admin/announcements or /admin/announcements/index
        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var data = await _db.Announcements
                                .AsNoTracking()
                                .Include(a => a.Faculty)
                                .OrderByDescending(a => a.CreatedAt)
                                .ToListAsync(ct);

            return View(data);
        }

        // GET: /admin/announcements/create
        [HttpGet("create")]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateFacultiesAsync(null, ct);
            return View(new Announcement());
        }

        // POST: /admin/announcements/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Announcement model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateFacultiesAsync(model?.FacultyId, ct);
                return View(model);
            }

            try
            {
                await _ann.CreateAsync(model, ct);
                TempData["SuccessMessage"] = "Announcement created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (OperationCanceledException)
            {
                ModelState.AddModelError(string.Empty, "Request was cancelled.");
                await PopulateFacultiesAsync(model?.FacultyId, CancellationToken.None);
                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the announcement.");
                await PopulateFacultiesAsync(model?.FacultyId, CancellationToken.None);
                return View(model);
            }
        }

        // GET: /admin/announcements/edit/{id}
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var x = await _ann.GetByIdAsync(id, ct);
            if (x == null) return NotFound();

            await PopulateFacultiesAsync(x.FacultyId, ct);
            return View(x);
        }

        // POST: /admin/announcements/edit/{id}
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Announcement model, CancellationToken ct)
        {
            if (model == null) return BadRequest();
            if (id != model.Id) return BadRequest("Mismatched announcement id.");

            if (!ModelState.IsValid)
            {
                await PopulateFacultiesAsync(model.FacultyId, ct);
                return View(model);
            }

            try
            {
                await _ann.UpdateAsync(model, ct);
                TempData["SuccessMessage"] = "Announcement updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (OperationCanceledException)
            {
                ModelState.AddModelError(string.Empty, "Request was cancelled.");
                await PopulateFacultiesAsync(model.FacultyId, CancellationToken.None);
                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the announcement.");
                await PopulateFacultiesAsync(model.FacultyId, CancellationToken.None);
                return View(model);
            }
        }

        // POST: /admin/announcements/delete/{id}
        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                await _ann.DeleteAsync(id, ct);
                TempData["SuccessMessage"] = "Announcement deleted successfully.";
            }
            catch (OperationCanceledException)
            {
                TempData["ErrorMessage"] = "Request was cancelled.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the announcement.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /admin/announcements/details/{id}
        [HttpGet("details/{id:int}")]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var x = await _ann.GetByIdAsync(id, ct);
            if (x == null) return NotFound();
            return View(x);
        }
    }
}
