using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Models
{
    public class ProgramSubject
    {
        public int ProgramSubjectId { get; set; }
        public string SubjectCode { get; set; }
        public string CourseId { get; set; }
        public string Name { get; set; }
        public string Credit { get; set; }

        public List<ParProDetail> ParProDetails { get; set; }
        public List<PreProDetail> PreProDetails { get; set; }
    }
}
