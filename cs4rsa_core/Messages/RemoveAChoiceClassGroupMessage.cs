using cs4rsa_core.BaseClasses;

namespace cs4rsa_core.Messages
{
    /// <summary>
    /// Publish Message này để loại bỏ một ClassGroup đã chọn
    /// </summary>
    public class RemoveAChoiceClassGroupMessage : Cs4rsaMessage
    {
        public new string Source;
        public RemoveAChoiceClassGroupMessage(string source) : base(source)
        {
            Source = source;
        }
    }
}
