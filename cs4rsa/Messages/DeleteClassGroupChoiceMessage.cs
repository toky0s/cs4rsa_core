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
    /// Message này được gửi đi khi bạn Bỏ chọn một ClassGroup nào đó.
    /// </summary>
    public class DeleteClassGroupChoiceMessage : Cs4rsaMessage
    {
        public new List<ClassGroupModel> Source;
        public DeleteClassGroupChoiceMessage(List<ClassGroupModel> source) : base(source)
        {
            Source = source;
        }
    }
}
