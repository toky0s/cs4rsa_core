using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.Models.Database;

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

        /// <summary>
        /// Major Subject được lựa chọn
        /// </summary>
        internal sealed class SelectMajorMsg : ValueChangedMessage<MajorSubjectModel>
        {
            public SelectMajorMsg(MajorSubjectModel mj) : base(mj)
            {

            }
        }
    }
}
