namespace My_Uni_Hub.Models.Pages
{
    public class ExamArchive
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public int Year { get; set; }

        public string Type { get; set; }
        public DateTime UploadDate { get; set; }

        public int? CourseId { get; set; }
        public Course Course { get; set; }
    }
}
