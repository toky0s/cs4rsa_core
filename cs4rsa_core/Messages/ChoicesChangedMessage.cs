using cs4rsa_core.BaseClasses;
using cs4rsa_core.Models;
using System.Collections.Generic;

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
