using CommunityToolkit.Mvvm.Messaging.Messages;

using Cs4rsa.Models;

namespace Cs4rsa.Messages.Publishers
{
    internal sealed class AutoVmMsgs
    {
        /// <summary>
        /// Hiển thị lớp đã chọn lên lịch
        /// </summary>
        internal sealed class ShowOnSimuMsg : ValueChangedMessage<CombinationModel>
        {
            public ShowOnSimuMsg(CombinationModel value) : base(value)
            {
            }
        }

        internal sealed class AddTreeItemMsg : ValueChangedMessage<ProgramSubjectModel>
        {
            public AddTreeItemMsg(ProgramSubjectModel value) : base(value)
            {
            }
        }


        /// <summary>
        /// Thêm vào tập kết quả một kết hợp
        /// </summary>
        internal sealed class AddCombinationMsg : ValueChangedMessage<CombinationModel>
        {
            public AddCombinationMsg(CombinationModel value) : base(value)
            {

            }
        }
    }
}
