using System.Collections.Generic;

namespace cs4rsa_core.Cs4rsaDatabase.Models
{
    public class ProgramSubject
    {
        public int ProgramSubjectId { get; set; }
        public string SubjectCode { get; set; }
        public string CourseId { get; set; }
        public string Name { get; set; }
        public int Credit { get; set; }

        public List<ParProDetail> ParProDetails { get; set; }
        public List<PreProDetail> PreProDetails { get; set; }
    }
}
