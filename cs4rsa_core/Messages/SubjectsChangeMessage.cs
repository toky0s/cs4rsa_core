using cs4rsa_core.BaseClasses;

using SubjectCrawlService1.Models;

using System.Collections.Generic;

namespace SubjectCrawlService1.Messages
{
    /// <summary>
    /// Message này được publish mỗi khi Search session có sự thay đổi về số lượng.
    /// </summary>
    public class SubjectsChangeMessage : Cs4rsaMessage
    {
        public new List<SubjectModel> Source;
        public SubjectsChangeMessage(List<SubjectModel> source) : base(source)
        {
            Source = source;
        }
    }
}
