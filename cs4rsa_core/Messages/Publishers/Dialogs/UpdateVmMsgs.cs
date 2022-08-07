using CommunityToolkit.Mvvm.Messaging.Messages;

namespace cs4rsa_core.Messages.Publishers.Dialogs
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
