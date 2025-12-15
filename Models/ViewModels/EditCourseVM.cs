using My_Uni_Hub.Models.Pages;
using System.ComponentModel.DataAnnotations;

public class EditCourseVM
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int CourseCode { get; set; }

    [Required]
    public string Semester { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public int FacultyId { get; set; }

    [Required]
    public int DepartmentId { get; set; }


}
