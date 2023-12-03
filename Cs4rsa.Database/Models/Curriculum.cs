using System.Collections.Generic;

namespace Cs4rsa.Database.Models
{
    public class Curriculum
    {
        public int CurriculumId { get; set; }
        public string Name { get; set; }
        public List<DbProgramSubject> DbProgramSubjects { get; set; }
    }
}
