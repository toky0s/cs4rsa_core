using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Cs4rsa.Messages.Publishers
{
    internal sealed class HomeVmMsgs
    {
        /// <summary>
        /// Gọi dialog cập nhật cơ sở dữ liệu môn học
        /// </summary>
        internal sealed class UpdateSubjectDbMsg : ValueChangedMessage<object>
        {
            public UpdateSubjectDbMsg(object value) : base(value)
            {
            }
        }
    }
}
