using cs4rsa_core.BaseClasses;

namespace cs4rsa_core.Messages
{
    /// <summary>
    /// Thông báo trạng thái update cơ sở dữ liệu môn học thành công.
    /// </summary>
    public class UpdateSuccessMessage : Cs4rsaMessage
    {
        public UpdateSuccessMessage(object source) : base(source)
        {
        }
    }
}
