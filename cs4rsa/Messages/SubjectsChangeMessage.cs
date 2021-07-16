using cs4rsa.BaseClasses;
using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Messages
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
