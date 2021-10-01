using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Models
{
    public class Curriculum
    {
        public int CurriculumId { get; set; }
        public string Name { get; set; }
        public ICollection<Student> Students { get; set; }
    }
}
