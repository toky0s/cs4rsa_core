using cs4rsa_core.BaseClasses;
using cs4rsa_core.Models;

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
