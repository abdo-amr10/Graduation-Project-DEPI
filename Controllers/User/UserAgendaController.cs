using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using My_Uni_Hub.Services.Interfaces;

[Authorize(Roles = "User,Admin")]
[Route("useragenda")]
public class UserAgendaController : Controller
{
    private readonly IUniversityAgendaService _agendaService;

    public UserAgendaController(IUniversityAgendaService agendaService)
    {
        _agendaService = agendaService;
    }

    // GET /useragenda
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var events = await _agendaService.GetAllEventsAsync();
        return View(events);
    }

    // GET /useragenda/details/5
    [HttpGet("details/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var eventItem = await _agendaService.GetEventByIdAsync(id);
        if (eventItem == null) return NotFound();
        return View(eventItem);
    }

    // GET /useragenda/upcoming
    [HttpGet("upcoming")]
    public async Task<IActionResult> Upcoming()
    {
        var events = await _agendaService.GetUpcomingEventsAsync(7);
        return View(events);
    }

    // GET /useragenda/bytype/{type}
    [HttpGet("bytype/{type}")]
    public async Task<IActionResult> ByType(string type)
    {
        if (string.IsNullOrEmpty(type)) return RedirectToAction(nameof(Index));
        var events = await _agendaService.GetEventsByTypeAsync(type);
        ViewBag.EventType = type;
        return View(events);
    }

    // GET /useragenda/bydate?startDate=2025-12-01&endDate=2025-12-10
    [HttpGet("bydate")]
    public async Task<IActionResult> ByDate(DateTime? startDate, DateTime? endDate)
    {
        if (!startDate.HasValue || !endDate.HasValue) return RedirectToAction(nameof(Index));
        var events = await _agendaService.GetEventsByDateRangeAsync(startDate.Value, endDate.Value);
        ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
        ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
        return View(events);
    }

    // GET /useragenda/byfaculty/3
    [HttpGet("byfaculty/{facultyId:int}")]
    public async Task<IActionResult> ByFaculty(int facultyId)
    {
        var events = await _agendaService.GetEventsByFacultyAsync(facultyId);
        return View(events);
    }

    // GET /useragenda/bydepartment/4
    [HttpGet("bydepartment/{departmentId:int}")]
    public async Task<IActionResult> ByDepartment(int departmentId)
    {
        var events = await _agendaService.GetEventsByDepartmentAsync(departmentId);
        return View(events);
    }

    // GET /useragenda/myevents/123
    [HttpGet("myevents/{studentId:int}")]
    public async Task<IActionResult> MyEvents(int studentId)
    {
        var events = await _agendaService.GetEventsByStudentAsync(studentId);
        return View(events);
    }
}
