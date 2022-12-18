using System.Collections.Generic;

namespace Cs4rsa.Services.SubjectCrawlerSvc.DataTypes
{
    /// <summary>
    /// Đối tượng chứa thông tin các môn tiên quyết và xong hành
    /// </summary>
    public class PreParContainer
    {
        public IEnumerable<string> PrerequisiteSubjects { get; set; }
        public IEnumerable<string> ParallelSubjects { get; set; }
    }
}
