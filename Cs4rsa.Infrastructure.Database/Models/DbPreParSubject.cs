using System.Collections.Generic;

namespace Cs4rsa.Database.Models
{
    /// <summary>
    /// Kho chứa thông tin các môn tiên quyết và song hành.
    /// </summary>
    public class DbPreParSubject
    {
        public int DbPreParSubjectId { get; set; }
        public string SubjectCode { get; set; }
        public List<PreProDetail> PreProDetails { get; set; }
        public List<ParProDetail> ParProDetails { get; set; }
    }
}
