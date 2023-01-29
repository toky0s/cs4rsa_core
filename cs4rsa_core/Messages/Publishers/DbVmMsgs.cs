using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Cs4rsa.Messages.Publishers
{
    internal sealed class DbVmMsgs
    {
        /// <summary>
        /// Refresh message yêu cầu cập nhật lại các dữ liệu được lấy DB.
        /// </summary>
        internal sealed class RefreshMsg : RequestMessage<DbVmMsgs>
        {
            public RefreshMsg() : base()
            {
            }
        }
    }
}
