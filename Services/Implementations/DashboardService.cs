using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using My_Uni_Hub.Models.Pages;
using My_Uni_Hub.Models.ViewModels.UserViewModel;
using My_Uni_Hub.Services.Interfaces;

namespace My_Uni_Hub.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly IAnnouncementService _announcementService;
        private readonly IOpportunityService _opportunityService; // <<-- added

        public DashboardService(
            IStudentService studentService,
            ICourseService courseService,
            IAnnouncementService announcementService,
            IOpportunityService opportunityService) // <<-- inject here
        {
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
            _announcementService = announcementService ?? throw new ArgumentNullException(nameof(announcementService));
            _opportunityService = opportunityService ?? throw new ArgumentNullException(nameof(opportunityService));
        }

        public async Task<DashboardViewModel> GetDashboardForUserAsync(string userId, CancellationToken ct)
        {
            // 1) Get student with related CourseStudents (your StudentService.GetByUserIdAsync already includes CourseStudents)
            var student = await _studentService.GetByUserIdAsync(userId, ct);

            if (student == null)
            {
                // return an empty/minimal VM to avoid null refs in the view
                return new DashboardViewModel
                {
                    StudentId = 0,
                    FullName = "Student",
                    Courses = new List<CourseCardVm>(),
                    Announcements = new List<AnnouncementVm>(),
                    Events = new List<SimpleEventVm>(),
                    Opportunities = new List<Opportunity>() // <-- ensure not null
                };
            }

            // 2) Get courses from student's navigation property (CourseStudents -> Course)
            var courses = new List<Course>();
            if (student.CourseStudents != null && student.CourseStudents.Any())
            {
                courses = student.CourseStudents
                                 .Where(cs => cs.Course != null)
                                 .Select(cs => cs.Course!)
                                 .ToList();
            }

            // 3) Map to ViewModel
            var vm = new DashboardViewModel
            {
                StudentId = student.Id,
                FullName = student.FullName,
                PhotoUrl = student.PhotoUrl,
                FacultyName = student.Faculty?.Name,
                AcademicYear = student.AcademicYear,
                Courses = courses.Select(c => new CourseCardVm
                {
                    Id = c.Id,
                    Title = c.Name,
                    Lecturer = c.Faculty?.Name ?? c.Department?.Name ?? "—",
                    ShortDesc = c.Description
                }).ToList(),

                Announcements = new List<AnnouncementVm>(), // fill later if you have AnnouncementService
                Events = new List<SimpleEventVm>(),
                Opportunities = new List<Opportunity>() // initialize
            };

            // 4) Fill announcements if service available
            if (_announcementService != null)
            {
                var latestAnn = await _announcementService.GetLatestAsync(5, ct); // get few latest
                vm.Announcements = latestAnn
                    .OrderByDescending(a => a.CreatedAt)
                    .Select(a => new AnnouncementVm
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Body = a.Body,
                        CreatedAt = a.CreatedAt
                    })
                    .ToList();
            }

            // 5) Fill opportunities from IOpportunityService
            try
            {
                // Get all via service (your service orders by PostedAt already, but ensure ordering here)
                var opportunities = await _opportunityService.GetAllAsync(ct);
                vm.Opportunities = (opportunities ?? Enumerable.Empty<Opportunity>())
                                    .OrderByDescending(o => o.PostedAt)
                                    // optionally limit here, or you can let the view take(3)
                                    //.Take(10)
                                    .ToList();
            }
            catch (Exception ex)
            {
                // don't fail the entire dashboard if opportunities query fails
                // optionally log the exception (inject logger if desired)
                vm.Opportunities = new List<Opportunity>();
            }

            return vm;
        }
    }
}
