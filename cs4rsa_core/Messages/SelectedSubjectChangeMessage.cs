using cs4rsa_core.BaseClasses;
using cs4rsa_core.Models;

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
