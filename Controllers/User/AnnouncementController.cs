using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using My_Uni_Hub.Models;
using My_Uni_Hub.Models.Pages;

namespace My_Uni_Hub.Controllers.User
{
    [Route("announcements", Name = "announcements")]
    public class AnnouncementsController : Controller
    {
        private readonly MyUniDbContext _db;

        public AnnouncementsController(MyUniDbContext db)
        {
            _db = db;
        }

        // GET /announcements
        [HttpGet("")]
        public async Task<IActionResult> Index(int? facultyId, CancellationToken ct)
        {
            IQueryable<Announcement> q = _db.Announcements.AsNoTracking();

            if (facultyId.HasValue)
                q = q.Where(a => a.FacultyId == facultyId.Value);

            q = q.Include(a => a.Faculty);


            var list = await q
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync(ct);

            var faculties = await _db.Faculties.AsNoTracking().OrderBy(f => f.Name).ToListAsync(ct);
            ViewBag.Faculties = faculties;
            ViewBag.SelectedFacultyId = facultyId;

            return View("UserAnnouncements", list);
        }

        // GET /announcements/details/{id}
        [HttpGet("details/{id:int}")]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var a = await _db.Announcements
                             .AsNoTracking()
                             .Include(x => x.Faculty)
                             .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (a == null) return NotFound();

            return View("AnnouncementDetails", a);
        }

        // GET /announcements/listjson?facultyId=2
        [HttpGet("listjson")]
        public async Task<IActionResult> ListJson(int? facultyId, CancellationToken ct)
        {
            var q = _db.Announcements
           .AsNoTracking()
           .AsQueryable(); 

            if (facultyId.HasValue)
                q = q.Where(a => a.FacultyId == facultyId.Value);

            q = q.Include(a => a.Faculty);

            var items = await q
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    Body = a.Body,   
                    Excerpt = (a.Body != null && a.Body.Length > 200)
                                ? a.Body.Substring(0, 200) + "..."
                                : a.Body,
                    CreatedAt = a.CreatedAt,
                    Faculty = a.Faculty != null ? a.Faculty.Name : null
                })
                .ToListAsync(ct);



            return Json(items);
        }
    }
}
