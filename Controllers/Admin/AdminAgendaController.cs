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
        [Route("admin/agenda")]
        public class AdminAgendaController : Controller
        {
            private readonly IUniversityAgendaService _agendaService;
        private readonly MyUniDbContext _context;

        public AdminAgendaController(IUniversityAgendaService agendaService , MyUniDbContext context)
            {
                _agendaService = agendaService;
                _context = context;
        }

            // GET: /admin/agenda
            [HttpGet("")]
            [HttpGet("/admin/agenda/index")]
            public async Task<IActionResult> Index()
            {
                var events = await _agendaService.GetAllEventsAsync();
                return View(events);
            }

            // GET: /admin/agenda/details/5
            [HttpGet("details/{id:int}")]
            public async Task<IActionResult> Details(int id)
            {
                var eventItem = await _agendaService.GetEventByIdAsync(id);
                if (eventItem == null)
                    return NotFound();

                return View(eventItem);
            }

            // GET: /admin/agenda/create
            [HttpGet("create")]
            public async Task<IActionResult> Create()
            {
            ViewBag.Faculties = new SelectList(await _context.Faculties.ToListAsync(), "Id", "Name");
            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "Name");

            return View();
            }

            // POST: /admin/agenda/create
            [HttpPost("create")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(UniversityAgenda agenda)
            {
                if (ModelState.IsValid)
                {
                    await _agendaService.CreateEventAsync(agenda);
                    return RedirectToAction(nameof(Index));
                }
            ViewBag.Faculties = new SelectList(await _context.Faculties.ToListAsync(), "Id", "Name");
            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "Name");
            return View(agenda);
            }

            // GET: /admin/agenda/edit/5
            [HttpGet("edit/{id:int}")]
            public async Task<IActionResult> Edit(int id)
            {
                var eventItem = await _agendaService.GetEventByIdAsync(id);
                if (eventItem == null)
                    return NotFound();
            return View(eventItem);
            }

            // POST: /admin/agenda/edit/5
            [HttpPost("edit/{id:int}")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, UniversityAgenda agenda)
            {
                if (id != agenda.Id)
                    return NotFound();

                if (ModelState.IsValid)
                {
                    var updated = await _agendaService.UpdateEventAsync(agenda);
                    if (updated)
                        return RedirectToAction(nameof(Index));

                    return NotFound();
                }

            return View(agenda);
            }

            // GET: /admin/agenda/delete/5
            [HttpGet("delete/{id:int}")]
            public async Task<IActionResult> Delete(int id)
            {
                var eventItem = await _agendaService.GetEventByIdAsync(id);
                if (eventItem == null)
                    return NotFound();

                return View(eventItem);
            }

            // POST: /admin/agenda/delete/5
            [HttpPost("delete/{id:int}")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var deleted = await _agendaService.DeleteEventAsync(id);

                TempData[deleted ? "SuccessMessage" : "ErrorMessage"]
                    = deleted ? "deleted :)" : "Fail to delete :(";

                return RedirectToAction(nameof(Index));
            }

            // GET: /admin/agenda/upcoming
            [HttpGet("upcoming")]
            public async Task<IActionResult> Upcoming()
            {
                var events = await _agendaService.GetUpcomingEventsAsync(7);
                return View(events);
            }

            // GET: /admin/agenda/by-date?startDate=2025-01-01&endDate=2025-01-10
            [HttpGet("by-date")]
            public async Task<IActionResult> ByDate(DateTime? startDate, DateTime? endDate)
            {
                if (!startDate.HasValue || !endDate.HasValue)
                    return RedirectToAction(nameof(Index));

                var events = await _agendaService.GetEventsByDateRangeAsync(startDate.Value, endDate.Value);

                ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
                ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");

                return View(events);
            }
        }
    }
