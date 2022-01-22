using cs4rsa_core.BaseClasses;
using cs4rsa_core.Models;
using System.Collections.Generic;

namespace cs4rsa_core.Messages
{
    /// <summary>
    /// Message này được gửi đi khi bạn Bỏ chọn một ClassGroup nào đó.
    /// Danh sách class group model được truyền vào hàm tạo là danh sách class group
    /// model hiện tại của Choice Session.
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
