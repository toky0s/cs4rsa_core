using System;

namespace Cs4rsa.Database.Models
{
    public class Keyword : IEquatable<Keyword>
    {
        public int KeywordId { get; set; }
        public string Keyword1 { get; set; }
        public string CourseId { get; set; }
        public string SubjectName { get; set; }
        public string Color { get; set; }
        public string Cache { get; set; }

        public int DisciplineId { get; set; }
        public Discipline Discipline { get; set; }
        public string SemesterId { get; set; }

        public bool Equals(Keyword other)
        {
            return other != null &&
                   KeywordId == other.KeywordId &&
                   Keyword1 == other.Keyword1 &&
                   CourseId == other.CourseId &&
                   SubjectName == other.SubjectName &&
                   DisciplineId == other.DisciplineId &&
                   SemesterId == other.SemesterId;
        }
    }
}
