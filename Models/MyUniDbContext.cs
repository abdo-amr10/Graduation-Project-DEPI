using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using My_Uni_Hub.Models.Pages;

namespace My_Uni_Hub.Models
{
    public class MyUniDbContext : IdentityDbContext<ApplicationUser>
    {
        public MyUniDbContext(DbContextOptions<MyUniDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<ExamArchive> ExamArchives { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<LostItem> LostItems { get; set; }
        public DbSet<UniversityAgenda> UniversityAgendas { get; set; }
        public DbSet<Opportunity> Opportunities { get; set; }
        public DbSet<DashboardNotification> DashboardNotifications { get; set; }
        public DbSet<CourseStudent> CourseStudents { get; set; } = null!; 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>().HasIndex(s => s.Email).IsUnique();
            modelBuilder.Entity<IdentityRole>().HasData(
                   new IdentityRole
                   {
                       Id = "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                       Name = "Admin",
                       NormalizedName = "ADMIN",
                       ConcurrencyStamp = "1"
                   },
                   new IdentityRole
                   {
                       Id = "2b3c4d5e-6f7g-8h9i-0j1k-2l3m4n5o6p7q",
                       Name = "User",
                       NormalizedName = "USER",
                       ConcurrencyStamp = "2"
                   }
               );

        

            modelBuilder.Entity<CourseStudent>(entity =>
            {
                entity.HasKey(e => new { e.CoursesId, e.StudentsId });

                entity.HasOne(e => e.Course)
                      .WithMany(c => c.CourseStudents)
                      .HasForeignKey(e => e.CoursesId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Student)
                      .WithMany(s => s.CourseStudents)
                      .HasForeignKey(e => e.StudentsId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
