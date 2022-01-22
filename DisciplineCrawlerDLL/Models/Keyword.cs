namespace DisciplineCrawlerDLL.Models
{
    public class Keyword
    {
        public string Keyword1 { get; }
        public int CourseId { get; }
        public string SubjectName { get; }
        public string Color { get; }
        public Keyword(string keyword1, int courseId, string subjectName, string color)
        {
            Keyword1 = keyword1;
            CourseId = courseId;
            SubjectName = subjectName;
            Color = color;
        }
    }
}
