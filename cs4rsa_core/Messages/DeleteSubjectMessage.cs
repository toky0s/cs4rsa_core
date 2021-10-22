using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.Models;
using SubjectCrawlService1.DataTypes;

namespace cs4rsa_core.Messages
{
    /// <summary>
    /// Message này được gửi đi khi một Subject bị xoá khỏi danh sách.
    /// </summary>
    public class DeleteSubjectMessage : Cs4rsaMessage
    {
        public readonly new SubjectModel Source;
        public DeleteSubjectMessage(SubjectModel source) : base(source)
        {
            Source = source;
        }
    }
}
