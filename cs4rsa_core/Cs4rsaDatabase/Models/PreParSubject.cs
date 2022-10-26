using System.Collections.Generic;

namespace cs4rsa_core.Cs4rsaDatabase.Models
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
