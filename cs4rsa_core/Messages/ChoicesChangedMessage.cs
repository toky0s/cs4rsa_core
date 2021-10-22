using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.Models;

namespace cs4rsa_core.Messages
{
    /// <summary>
    /// Message này được gửi đi khi có sự thay đổi trong danh sách các class group đã chọn.
    /// </summary>
    public class ChoicesChangedMessage : Cs4rsaMessage
    {
        public new List<ClassGroupModel> Source;
        public ChoicesChangedMessage(List<ClassGroupModel> source) : base(source)
        {
            Source = source;
        }
    }
}
