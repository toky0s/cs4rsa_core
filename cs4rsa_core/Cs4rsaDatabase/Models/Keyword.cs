using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cs4rsa.Cs4rsaDatabase.Models
{
    public class Keyword
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int KeywordId { get; set; }
        public string Keyword1 { get; set; }
        public int CourseId { get; set; }
        public string SubjectName { get; set; }
        public string Color { get; set; }
        public string Cache { get; set; }

        public int DisciplineId { get; set; }
        public Discipline Discipline { get; set; }
    }
}
