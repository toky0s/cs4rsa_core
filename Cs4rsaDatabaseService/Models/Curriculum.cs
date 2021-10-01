using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Models
{
    public class Curriculum
    {
        public int CurriculumId { get; set; }
        public string Name { get; set; }
        public List<Student> Students { get; set; }
    }
}
