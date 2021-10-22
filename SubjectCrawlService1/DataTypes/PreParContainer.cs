using System.Collections.Generic;

namespace SubjectCrawlService1.DataTypes
{
    /// <summary>
    /// Đối tượng chứa thông tin các môn tiên quyết và xong hành
    /// </summary>
    public class PreParContainer
    {
        public List<string> PrerequisiteSubjects { get; set; }
        public List<string> ParallelSubjects { get; set; }
    }
}
