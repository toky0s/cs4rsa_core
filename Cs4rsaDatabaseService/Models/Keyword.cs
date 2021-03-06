using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cs4rsaDatabaseService.Models
{
    public class Keyword
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int KeywordId { get; set; }
        public string Keyword1 { get; set; }
        public ushort CourseId { get; set; }
        public string SubjectName { get; set; }
        public string Color { get; set; }

        public int DisciplineId { get; set; }
        public Discipline Discipline { get; set; }
    }
}
