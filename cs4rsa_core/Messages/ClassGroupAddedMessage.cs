using cs4rsa_core.BaseClasses;
using SubjectCrawlService1.Models;

namespace cs4rsa_core.Messages
{
    /// <summary>
    /// Message này được publish khi người dùng thêm một ClassGroup vào danh sách chọn
    /// </summary>
    public class ClassGroupAddedMessage : Cs4rsaMessage
    {
        public new ClassGroupModel Source;
        public ClassGroupAddedMessage(ClassGroupModel source) : base(source)
        {
            Source = source;
        }
    }
}
