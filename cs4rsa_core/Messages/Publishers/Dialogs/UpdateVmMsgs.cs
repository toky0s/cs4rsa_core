using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.ViewModels.Database;

namespace Cs4rsa.Messages.Publishers.Dialogs
{
    internal sealed class UpdateVmMsgs
    {
        /// <summary>
        /// Cập nhật cơ sở dữ liệu môn học thành công
        /// </summary>
        internal sealed class UpdateSuccessMsg : RequestMessage<DbViewModel>
        {
            public UpdateSuccessMsg()
            {
            }
        }
    }
}
