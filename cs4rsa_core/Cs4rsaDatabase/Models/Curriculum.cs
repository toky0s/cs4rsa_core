using System.Collections.Generic;

namespace cs4rsa_core.Cs4rsaDatabase.Models
{
    public class Curriculum
    {
        public int CurriculumId { get; set; }
        public string Name { get; set; }
        public List<Student> Students { get; set; }
    }
}
