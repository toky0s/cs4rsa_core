using System.Collections.Generic;

namespace Cs4rsaDatabaseService.Models
{
    public class Curriculum
    {
        public int CurriculumId { get; set; }
        public string Name { get; set; }
        public List<Student> Students { get; set; }
    }
}
