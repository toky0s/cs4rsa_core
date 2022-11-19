using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Cs4rsa.Messages.Publishers.Dialogs
{
    internal sealed class UpdateVmMsgs
    {
        /// <summary>
        /// Cập nhật cơ sở dữ liệu môn học thành công
        /// </summary>
        internal sealed class UpdateSuccessMsg : ValueChangedMessage<object>
        {
            public UpdateSuccessMsg(object value) : base(value)
            {
            }
        }
    }
}
