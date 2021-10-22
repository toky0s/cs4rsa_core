using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa_core.Models;
using cs4rsa_core.BaseClasses;
using SubjectCrawlService1.DataTypes;

namespace cs4rsa_core.Messages
{
    /// <summary>
    /// Message này được gởi đi khi người dùng click vào một Subject khác trên danh sách.
    /// </summary>
    public class SelectedSubjectChangeMessage : Cs4rsaMessage
    {
        public new SubjectModel Source;
        public SelectedSubjectChangeMessage(SubjectModel source) : base(source)
        {
            Source = source;
        }
    }
}
