using cs4rsa_core.BaseClasses;

namespace cs4rsa_core.Messages
{
    /// <summary>
    /// Publish Message này để loại bỏ một ClassGroup đã chọn
    /// 
    /// classGroupName: Tên class group cần loại bỏ
    /// </summary>
    public class RemoveChoicedClassMessage : Cs4rsaMessage
    {
        public new string Source;
        public RemoveChoicedClassMessage(string classGroupName) : base(classGroupName)
        {
            Source = classGroupName;
        }
    }
}
