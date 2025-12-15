using System.ComponentModel.DataAnnotations;

namespace My_Uni_Hub.Models.Pages
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? CourseId { get; set; }
        public Course Course { get; set; }
        public ICollection<Answer> answers { get; set; } = new List<Answer>();
        public int? StudentId { get; set; }
        public Student Student { get; set; }

    }
}
