namespace Cs4rsa.Database.Models
{
    public class Keyword
    {
        public int KeywordId { get; set; }
        public string Keyword1 { get; set; }
        public string CourseId { get; set; }
        public string SubjectName { get; set; }
        public string Color { get; set; }
        public string Cache { get; set; }

        public int DisciplineId { get; set; }
        public Discipline Discipline { get; set; }
    }
}
