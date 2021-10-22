using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Models
{
    /// <summary>
    /// Một môn học có nhiều môn song hành.
    /// </summary>
    public class ParProDetail
    {
        public int ProgramSubjectId { get; set; }
        public ProgramSubject ProgramSubject { get; set; }
        public int PreParSubjectId { get; set; }
        public PreParSubject PreParSubject { get; set; }
    }
}
