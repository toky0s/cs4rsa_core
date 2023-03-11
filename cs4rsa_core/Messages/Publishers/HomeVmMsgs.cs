using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.ViewModels;

namespace Cs4rsa.Messages.Publishers
{
    internal sealed class HomeVmMsgs
    {
        /// <summary>
        /// Gọi dialog cập nhật cơ sở dữ liệu môn học
        /// </summary>
        internal sealed class UpdateSubjectDbMsg : RequestMessage<HomeViewModel>
        {
            public UpdateSubjectDbMsg() : base()
            {
            }
        }
    }
}
