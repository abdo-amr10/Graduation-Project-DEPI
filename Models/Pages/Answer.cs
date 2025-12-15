namespace My_Uni_Hub.Models.Pages
{
    public class Answer
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        //public bool IsAccepted { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int? StudentId { get; set; }
        public Student Student { get; set; }

    }
}
