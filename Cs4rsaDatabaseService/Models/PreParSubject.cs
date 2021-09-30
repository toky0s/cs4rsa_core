using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Models
{
    /// <summary>
    /// Kho chứa thông tin các môn tiên quyết và song hành.
    /// </summary>
    public class PreParSubject
    {
        public int PreParSubjectId { get; set; }
        public string SubjectCode { get; set; }
        public List<PreProDetail> PreProDetails { get; set; }
        public List<ParProDetail> ParProDetails { get; set; }
    }
}
